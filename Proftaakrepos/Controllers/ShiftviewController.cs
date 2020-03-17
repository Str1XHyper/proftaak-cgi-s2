using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proftaakrepos.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using ClassLibrary.Classes;

namespace Proftaakrepos.Controllers
{
    public class ShiftviewController : Controller
    {
        public IActionResult Incoming()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }

        public IActionResult CreateRequest()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }


        public IActionResult HandleRequest(string UserID, int TradeID)
        {
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"Update TradeRequest Set Status = 1 Where TradeId = {TradeID} ";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                return View("Index");
            }

            cmd.CommandText = $"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID}";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                return View("Index");
            }
            return View("Index");
        }

        public IActionResult Block(string UserID, int TradeID, string DisabledIds)
        {
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"Update TradeRequest Set DisabledIds = '{UserID} {DisabledIds}'Where TradeId = {TradeID}";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                return View("Index");
            }

            return View("Index");
        }
    }
}