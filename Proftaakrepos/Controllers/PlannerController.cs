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
using Models.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using CookieManager;
using ClassLibrary.Planner;

namespace Proftaakrepos.Controllers
{
    public class PlannerController : Controller
    {
        private static string userId;
        private static string rol;
        private List<EventModel> eventList;
        private AgendaManager agendamanager = new AgendaManager();
        private EventManager eventmanager = new EventManager();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
        }
        #region Views
        //[UserAccess("","Rooster")]
        public IActionResult Agenda()
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string[] loggedUserData = agendamanager.GetLoggedInUserData(var);
            rol = loggedUserData[0];
            AgendaViewModel viewdata = agendamanager.SetAgendaViewModel(loggedUserData[1]);
            ViewData["colours"] = agendamanager.GetThemeColours();
            ViewData["verlof"] = agendamanager.GetVerlofCount(loggedUserData[0]);
            ViewData["rol"] = rol;
            ViewData["userId"] = loggedUserData[1];
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
            string[] userIdArray = eventmanager.GetSelectedUsersEvents(userId, SendUserId);
            List<string[]> events = eventmanager.GetEventData(userIdArray);
            List<EventModel> eventList = eventmanager.FillEventModelList(events, userIdArray, rol);
            ViewData["rol"] = rol;
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