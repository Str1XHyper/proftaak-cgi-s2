using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Models;
using Models.Agenda;
using Proftaakrepos.Authorize;
using CookieManager;
using Models.Authentication;

namespace Proftaakrepos.Controllers
{
    public class PlannerController : Controller
    {
        private static string userId;
        private static string rol;
        private List<EventModel> eventList;
        private readonly ICookieManager _cookieManager;
        public PlannerController(ICookieManager cookieManager)
        {
            _cookieManager = cookieManager;
        }
        #region Views
        [UserAccess("","Rooster")]
        public IActionResult Agenda()
        {
            string var = _cookieManager.Get<CookieModel>("BIER.User").Identifier;
            ViewData["UserInfo"] = var;
            string[] verlof = null;
            string[] themeColours = SQLConnection.ExecuteSearchQuery($"select * from ColorScheme").ToArray();
            string[] loggedUserData = SQLConnection.ExecuteSearchQuery($"Select Rol,UserId From Werknemers Where AuthCode = '{var}'").ToArray();
            userId = loggedUserData[1];
            rol = loggedUserData[0];
            if (rol.ToLower() == "roostermaker")
            {
                verlof = SQLConnection.ExecuteSearchQuery($"Select Count(*) From Verlofaanvragen").ToArray();
            }
            ViewData["colours"] = themeColours;
            ViewData["rol"] = rol;
            ViewData["verlof"] = verlof;
            ViewData["userId"] = userId;
            AgendaViewModel viewdata = new AgendaViewModel(userId);
            string[] roosterData = SQLConnection.ExecuteSearchQuery($"Select Rooster.*, Werknemers.Voornaam From Rooster INNER JOIN Werknemers ON Werknemers.UserId = Rooster.UserId").ToArray();
            List<EventModel> modelList = new List<EventModel>();
            for (int i = 0; i < roosterData.Length; i += 10)
            {
                EventModel model = new EventModel();
                model.eventId = Convert.ToInt32(roosterData[i]);
                model.userId = roosterData[i + 1];
                model.title = roosterData[i + 2];
                model.description = roosterData[i + 3];
                model.startDate = Convert.ToDateTime(roosterData[i + 4]);
                model.endDate = Convert.ToDateTime(roosterData[i + 5]);
                model.themeColor = roosterData[i + 6];
                model.isFullDay = Convert.ToInt32(roosterData[i + 7]);
                model.voornaam = roosterData[i + 9];
                viewdata.eventList.Add(model);
            }
            string[] userData = SQLConnection.ExecuteSearchQuery($"Select UserId, Voornaam, Tussenvoegsel, Achternaam, Rol From Werknemers").ToArray();
            for (int i = 0; i < userData.Length; i += 5)
            {
                UserViewModel usermodel = new UserViewModel(userData[i], userData[i + 1], userData[i + 2], userData[i + 3], userData[i + 4]);
                viewdata.userList.Add(usermodel);
            }
            string _authCode = HttpContext.Session.GetString("UserInfo");
            return View(viewdata);
        }
        #endregion
        #region Data Logic
        public void DeleteEvent(int EventId)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Rooster WHERE EventId = {EventId}");
        }
        public ActionResult GetEventInfo(int EventId)
        {
            List<string> eventData = SQLConnection.ExecuteSearchQuery($"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Werknemers.UserId = Rooster.UserId Where EventId = {EventId}");
            var start = DateTime.Parse(eventData[4]);
            var end = DateTime.Parse(eventData[5]);
            eventData[4] = start.ToString("yyyy-MM-dd'T'HH:mm");
            eventData[5] = end.ToString("yyyy-MM-dd'T'HH:mm");
            return Json(eventData);
        }
        [HttpPost]
        public void CreateEvent(EventModel newmodel)
        {
            newmodel.userId = newmodel.userId.Substring(0, newmodel.userId.Length - 1);
            string[] userIdArray = newmodel.userId.Split(",");
            if (ModelState.IsValid)
            {
                if (newmodel.eventId > 0)
                {
                    HandleEditEventRequest(newmodel);
                }
                else
                {
                    HandleEventRequest(newmodel, userIdArray);
                }
            }
        }
        public void HandleEditEventRequest(EventModel emdb)
        {
            emdb.userId = emdb.userId.Substring(0, emdb.userId.Length);
            string[] userIdArray = emdb.userId.Split(",");
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Rooster WHERE EventId = {emdb.eventId}");
            string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
            for (int i = 0; i < userIdArray.Length; i++)
            {
                if (i > 0)
                {
                    sqlquery += ",";
                }
                sqlquery += $"('{userIdArray[i]}', '{emdb.title}', '{emdb.description}', '{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.themeColor}', '{(emdb.isFullDay)}', '{(emdb.isPending ? 1 : 0)}')";
            }
            SQLConnection.ExecuteNonSearchQuery(sqlquery);
        }
        [HttpPost]
        public IActionResult HandleEventRequest(EventModel emdb, string[] useridArray)
        {
            string userId = "0";
            string var = HttpContext.Session.GetString("UserInfo");
            if (emdb.userId != "0")
            {
                userId = emdb.userId;
            }
            if (userId == "-1")
            {
                int userCount = Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"Select Count(UserId) From Werknemers")[0]);
                for (int i = 0; i < userCount; i++)
                {
                    SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Rooster (UserId,Subject,Description,Start,End,ThemeColor,IsFullDay,IsPending) VALUES ('{i}','{emdb.title}','{emdb.description}','{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.themeColor}','{(emdb.isFullDay)}','{(emdb.isPending ? 1 : 0)}')");
                }
            }
            else
            {
                string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
                for (int i = 0; i < useridArray.Length; i++)
                {
                    if (i > 0)
                    {
                        sqlquery += ",";
                    }
                    sqlquery += $"('{useridArray[i]}', '{emdb.title}', '{emdb.description}', '{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.themeColor}', '{(emdb.isFullDay)}', '{(emdb.isPending ? 1 : 0)}')";
                }
                SQLConnection.ExecuteNonSearchQuery(sqlquery);
            }
            return RedirectToAction("CreateEvent", "Planner");
        }
        [HttpGet]
        public IActionResult FetchAllEvents(string SendUserId)
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string _userId = "0";
            string[] userIdArray = new string[0] ;
            if (SendUserId == null || SendUserId == "0")
            {
                _userId = userId;
            }
            else
            {
                SendUserId = SendUserId.Substring(0, SendUserId.Length - 1);
                userIdArray = SendUserId.Split(",");
            }
            ViewData["rol"] = rol;
            eventList = new List<EventModel>();
            List<string[]> events;
            if (SendUserId == "-1")
            {
                events = SQLConnection.ExecuteSearchQueryWithArrayReturn($"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Rooster.UserId = Werknemers.UserId");
            }
            else if (SendUserId == "0")
            {
                events = SQLConnection.ExecuteSearchQueryWithArrayReturn($"select * from Rooster WHERE UserId = '{_userId}'");
            }
            else
            {
                string sqlquery = $"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Rooster.UserId = Werknemers.UserId";
                for (int i = 0; i < userIdArray.Length; i++)
                {
                    if (i == 0)
                    {
                        sqlquery += " WHERE ";
                    }
                    if (i > 0)
                    {
                        sqlquery += " OR ";
                    }
                    sqlquery += $"Rooster.UserId = '{userIdArray[i]}'";
                }

                events = SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlquery);
            }
            foreach (string[] e in events)
            {
                EventModel em = new EventModel();
                em.eventId = Convert.ToInt32(e[0]);
                em.userId = e[1];
                if (SendUserId == "-1" || SendUserId == null || rol == "Roostermaker")
                {
                    em.title = e[9] + " - " + e[2].ToString();
                }
                else
                {
                    em.title = e[2].ToString();
                }
                em.description = e[3].ToString();
                em.startDate = Convert.ToDateTime(e[4]);
                em.endDate = Convert.ToDateTime(e[5]);
                em.themeColor = e[6].ToString();
                em.isFullDay = Convert.ToInt32(e[7]);
                em.isPending = Convert.ToBoolean(e[8]);
                eventList.Add(em);
            }

            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return Json(eventList);
        }
        public void UpdateAgendaTimes(DateTime startTime, DateTime endTime, int EventId, bool allDay)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set Start = '{startTime.ToString("yyyy/MM/dd HH:mm")}',End = '{endTime.ToString("yyyy/MM/dd HH:mm")}',IsFullDay = '{Convert.ToInt32(allDay)}' Where EventId = {EventId}");
        }
        public void UpdateAllDay(int EventId, bool allDay)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set IsFullDay = '{Convert.ToInt32(allDay)}' Where EventId = {EventId}");
        }
#endregion
    }
}
#region Easteregg
//     IS NOT BUG, IS FEATURE.                                                                                                                                                                                                
//,,,,,,,,,,,,,,,,,,,******,,******************,,,**,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,****.*,.*..  .. .. , .......,,,,,,,,,,,,,,,,,,,,,,,,,.........................................................
//,,,,,,,,,,,,,,,,**********************************,*,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,*******,.,.  .*///,,//**, ..  .,...,,,,,,,,,,,,,,,,,,,,,,,,,.,.....................................................
//,,,,,,,,,,,,,,,*************************************,,,,,,,,,,,,,,,.,,,,,,,,,,,,,,,,*******,..,./&%#((((((((((((//, .,..,,*(*...,,,**********,,,,,,,,...................................................
//,,,,,,,,,,,,****************************************,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,********,..*&&%%#((((((((((((((((, ,,,,,&&@&&&&@@@@@&&%#/, .,*,,,,,...........................,......................
//,,,,,,,,,,,*****************************************,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,***********%&%%###((((((((((((((((  .,,*,&%%&&&&&&&&&&&&&@(/,**,,,,,.....................,,,,,,,,,,,,,,,,,,,,.,......
//,,,,,,,,,,,*****************************************,*,,,,,,,,,,,,,,,,,,,,,,,,,,,**********..,&&(/**(((((/*    .,(#(# ...,,,.%%#%&@&&&&&&&&&@(/,***,,,,...................,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//,,,,,,,,,,,*********************/********************,,,,,,,,,,,,,,,,,,,,,,,,,,,**********/ .&&#(/*,.,((/#, .,.*((((/* . .,,    .,/(%%&@@@@@@(/*****,,,,..................,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//,,,,,,,,**********************//*********************,,,,,,,,,,,,,,,,,,,,,,,,,,,,**********(&(&(/*..,(&/*(/...*/(*//##    ...##(*,.,,***,,,*,.,/****,,,,,.............,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//,,,,,,,,*******************/////*/*/*******************,,,,,,,,,,,,,,,,,,,,,,,************/*.@%%%#/(%##((/#(*,,/(((((#.##(., ,&&###(/***,. *..,******,,,,.............,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//,,,,,,,*******************//*****/((###****************,,,,,,,,,,,,,,,,,,,,,,,,************* %&(//((/##/###..*(((((((((( ,(( .##(%&&%#(#(/*/..,//****,,,,,,...........,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//,.,*,,,,,,,****************/((((((**//(/**************,,,**.**,*****,,,,         .********** &/%%%###(,.,.,/((((((((((/*(#*  ./,*/((((#%%%%##*,/******,,,,,...........,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//../*,*//*//,*//**********/////(/*,.....((/************,,,**.,,,******,,. /**,///* *******.#(..&&%%%((......*(/(((((((..,    . //.,,,,,**///#(/./******,,**.,,.........,,,,,,,,,,,,,,,,,********,,,,*****
//****,.*.//// ,* /********/(/*************/*************,,**.,,,******,,. **** / * /*****, (/(  #&%%##/,,,*/(((((#((((,  .      **/////**,*,(,,./***,(.**./...,,.......,,,,,,,,,,,,,,,***************,   
// .,*,.///////((/(,*****/(((((((((/,......./************,,***,*,******,,. ****//*/ *****/.(/((.. %&&###(((/////((((((,,     .....,*/*////((/#//./***, (/,,,,,,,*,......,,,,,,,,,,,,,,,******,,*****,,,   
//****,.*///**,,,./..*,              ......  */***********,************,,. */.//**/ *****/ /// .,  .,%#(*/*/*******//((    ..     .,,,,,...,,(.*./****.(((**,,*,,,.......,,,,,,,,,,**********..,,.    .   
//*.  *..//*/*,**.*(,/,   ..........****. ...*/***********,*****,******,,. ,...,,,, /*****.(/ ., ..,.*&%(/,******//(((*      ..   .,,..... ../.,********/,*,****,,....  .,,  .  .. ..,.   . .,,. ..   .,,.
//,,,,*. ,. ,****//*//,   ...................*/***********,*****,******,,. ,....,,, /***/..((* ,.*... @&%#////////((((.     .  .....,,,,,,***(**(#//***/ ,*,**/,,,,..   .,.  .  ......  ... ...  .... ..  
//....,,. .......  .**,   ..........,,.......,/*************/**********,,, ........,/****,,,,. ,,..,,.(&%#//(///((((/,         ...,*//*///***/,**/*///// ..,****,,,..  .......  .....................     
//...,,,,,,,,,,,****,,.    ..................,/**************,,,,,,,,,*,,,,,,*****************/* .,    &&#//(((((((((/....      . ,*/******,,(.**//**.././**//*,**,..      ...  ...   .. ....  ......     
//,,,,,,,,,,,,******,,.    ..................,/***************,*,,,,,,****,,*****************,...////*  &&(((/((((((((*,/.    ....,.****(##%%%%**//*/.,/ *..(* /*,,..   (#(  .  ...%%#   ...##(    .  /** 
//,,,,,,,,,,*******,,,.   .................,.,/********************,,,,,,.... .              ..,,***.. .*&(((((((((***/#     .,,.,***//**/@&((##/..((/.   .///,*,*,..      ...  ..      ....,,. .  . .,,  
//,,,,,,,,,********,,,,   ..........,,,,,,,,,./********.*//////*((/*///,,*///**/*/*///*/(%(**,**((. . .*%&(((#%##(***%/ .   ..*,.*. ..,,,,,,.,##*..///,*(*.**..,,.,,.       .   ..  .*******/////**,,     
//,,,,,,,,,,*******,,,,    .........*%#(# ..../********.,/////(/**(///.//(/****/(,.%%#  ,(*(**/( . . *&%%%#/(&%%%#(%%, .   ,* .* .. ,,,,**,,*,...*,**/,*,*,* ..,*,,..   ... ..  ...******,,,,   *.    ..,.
//,,,,*,*//*,,,***,,,,,   ...........  ....,../********..(/...(//((////,**/* ,(,*(*,,..(/,**(  .... ,&@.%&%%%%%%%#(%* .   *..,..,,,,,.,,,,,***,,, ./**,,,*,,.*,/,,,..   ..        .*,,,,**,**  ,(%, . *///
//.**,***,**,,,,,,,,,,,      ................./********, (//(/,//((,**//((/,.**/****,((((.(*..(.....&&/.*%%%%%(/(#%   .   ,  ,,*,,,,*,,*,,,,/**,,.,,//,,**,.,,..,.,..      *,*(///(///*. ..,,  .*#  , .  ,
//.**,***,,*..,/******,    .................../*******,, (//.//  (/(,/*(**(.*# *. ..((*/*,/##   ...*&&@&%#(((/#%##&         .,,,*,,,,/*.,.,,,,,***.   * .  *..,  ,,.       *,,,,(/////*, ..,,  ...,  .....
//.,*.   ..*,**,/***,,,    ................,../*******,,./(//*/((///(/.**(,.**,*/, #(# (, (/( ,,.. #&&@&%#((/%%(#%#      ....,,//...,,,,*,,,.,,,*,,,  , ..*(*,,,%#((//*,** ,,,,,,.////,, ..,,,,..,,,,,.   
//.,,,,,**,.**,  ,*,*,,   ....................********,,.* (///*/*,*/((/**//,*(****/(*(* / (/.  ,.. %%%&&%((%#(#%&#.     .,. **/..,,...,,,, /*,*,,,,,  . ...(#(#((((((*//***,.,.,.,..*/,,..,***,,,,...    ,
//(/,,,,***.***/*  ,**,  .......,,,....,,..,..*********,.,(///*,//.,////**,.,***,( (( *,,.*/  #....%%(#&#%#/#%#&%.. ... *., ,.,....*,..,,,*(..,,*.., .. ...**..,,,,,..,,,//**,,*//(*,.. ...,,,,.,/*///****
// ,***,*/**/*,/%/..,,,....,*...,/*../(//.....**/(/,,***,./*//////,.,***,///*//*,..(/,,,,***.*/*,.,*/*/#*,/%#///(*......*.,.,,,*/,,**,.,,,****,..., ,.....,,,,...... .,,,//,,****..,,. ..,/ . .,.  ..,//, 
//...*****/*/##(((##/.#&##(#&...&####%%**,,,#@%####&*,***,(&&%%#%%%%%%##&@%%%%&#**@%###%&&#&&###%%%%%##&%&&%%%%%%%%%####@,,@%#####&#,.,,&%######&**...,&##%%#&/.. ...&######&*#(&(((#&, */#/,  ..,,      .
//.,**,,** //***,,,.,/&%%%%#,,,/&&%%&,,*,,,&&&&&&&%&,***/&&&&&&@&&&&&&&%&&&&&&*#&&&&&&@,(&&&&&&@&&&@&&&*/&&&%&@&&&&&&&&&&*/&&&&&&&&,,,*&&&&&&&&/*,,*,&&&&&&&%&@..../%&&%%%%%&*,#&%%%&..  ..,*(#%%%%%%%%%%/
//..,,*,/ .**. .,*//*%&%%%&%%&&&&&&&%*.,,#@&&&&(&&&&#,,&&&&&&%//**/.*(/*&&&&&&&&&&&&#,,&@&&&&&&&&@@@&&&(&@&&&&%##%&&&&&&&/&&&&&&&&&%,%&&&&&&&&&*,,,(&&&&&#&&&&%...,#&&&&&&&&&&*&&&%%(.............    ., .
//...* ((((*.  ...,,,(%&&&&@@&@@@@@&&%.,*&@@&&%*/@@@&&&%&@&&&&*/,*/////*&&@@@@@@&/&,,,,&@@@@@@@@@@@@@&%%#(@@@@@@@@@@&@&#&,&&@&&&@&@&@@@@@@&&@@%&..,@@@@&&.*@@@&&&.,,&&@&&&#@@@&&@@@@%( ........ ..   . .,  
/////((/,  ., .....,(,,,,#,*&%#,**,/,*/*,***,,,,*****/*/***/,,.......,#&,**,%,****#*,,#,***(***//////*(%***,%((%/***#&,*/%,***&(,******//****#*,&#,,**********,(*,*(,*,*,*#,*****,,.,,.....               
//.,*,  ,,.........%%%%%&,,,,&&%%%%*%&&&&&(*#%((%@&&&&&&&&&&&&&&&&&&&#&&&&&%/%@@&&&&&.%@@&&&&&&&&&&&&#&&&&&@**./@&&&&&#.&&&&&&/&@&&&&&&#@@&&&&&&&&&&%*//(#@&&&&&(,@&&%%#,.(&&%%%%%&.                      
//...*//,........,*&&%%&,.,,/@&&%%&&&&&&&,...,*/#@&&&&(,#@@&&&&&&&&&((@@&&@.**(&&&&&&,,/%@@&&&&&&&&&.(&@&&&./*..(@&&&&%(@@@&&&/#@@&&&,*&@&&&(#@@&&&(.,,,,,(@&&&&#/@@&&&*...(@&&%%&* ...                   
//...,,  .........,*///(.,,,,**/(((/((((((((/,/(((###,......,.,,..,,,,,,**((/*/**......./((**,*,**(*/#((%(#%(#..*&#//(((((////******,**,,**,*,,,*,,.,,.,,,,,,,,,,,,,,,,,,,,,,,,,,,...                ...,,
//... ,*,  ...............*//,...,,***/((/*,..#(/****(%....,,,..*(/,*(((/*.//,,****,.,&%####*   ((&%%&/%#&@#*,.&(**,***/* .*/*******,*,.,*. ,,,.,,.,...,.,,.,,......................              .,..,***
//#%#////**   ...............   ..,/(,     .. ,#(***/ ./ .  ,&(.,,.,/((  /**//,/,.  .&#((##%%%#(((/*,*.*/#%%, &**///*,,* . * ***,,**,*,****,  ,**,.,.,.,...........................   .       .     .     
//,   /((//**.  ..........................   ./,/,,..., .... .(%%/.   *,..*.,*/,/##(/*%#(#####%%#%%######,.,*,,#.,,...   /. ,,.,,,,,,,,,,,//****,.,...,,, ........................   .        *.          
//.*,  .(((/(      ..    .................... *,.. .*,..........*, ..  ..,,.., #%##(((/**(##(###########/(,(,/./* ,,.... ,*,*   ,,,,..,, .,*/**/(*.,,..,,, ......................             ,.          
//. */.  ,##  *,  ,,,,,,,,,,,,,..    ...........  ..  ..... .*,  ......&% .** (##(((((((#((/*/(((((((((( ,.#.* *., *,....,,.,,,.  .,..,,,.  .**,.. ....  ,........ ..............             .,          
//** ,%/.        ,,,,,,,,,,,,,,,,,,,,,,,*      ...,.  ...,,.  ..  .. .....,,. .((//////(/(#####((((((((,.,,,,,...,  *,.,,,,,,(/,..  ( , ,,,.  ,  .  ... /, ......  ..............             .*    
#endregion