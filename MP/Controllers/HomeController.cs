﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Microsoft.SqlServer.Server;
using MP.Data.Repository;
using MP.Data.Service;
using MP.Model.Models;
using MP.Model.SearchModels;
using MP.Models;
using Newtonsoft.Json;

namespace MP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ITripService tripService { get; set; }
        private IPassengerService passengerService { get; set; }
        private IItemService itemService { get; set; }
        public HomeController(ITripService tripService, IPassengerService passengerService, IItemService itemService)
        {
            this.tripService = tripService;
            this.passengerService = passengerService;
            this.itemService = itemService;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("TripContent", "Home", new { tripname = "ma" });
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserModel user, string returnUrl)
        {
            if (user.UserName == ConfigurationManager.AppSettings.Get("UserName") && user.Password == ConfigurationManager.AppSettings.Get("Password"))
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                FormsAuthentication.RedirectFromLoginPage(user.UserName, false);
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("TripContent", "Home", new { tripname = "ma" });
            }
            else
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View("Login", user);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TripContent(TripName tripName)
        {
            var trip = new Trip { TripName = tripName };
            return View(trip);
        }

        public ActionResult GetPassenger(TripModel tripModel)
        {
            var passengers = passengerService.GetPassengers(Mapper.Map<TripModel, Trip>(tripModel)).OrderBy(a => a.Town).Select(Mapper.Map<Passenger, PassengerModel>);
            return Json(passengers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTripDepartureTime(TripName tripName)
        {
            var maList = new List<object>
                 {
                     new {Text = "3:00", Value = DepartureTime.C300.ToString()},
                     new {Text = "5:30", Value = DepartureTime.C530.ToString()},
                     new {Text = "7:30", Value = DepartureTime.C730.ToString()},
                     new {Text = "9:30", Value = DepartureTime.C930.ToString()},
                     new {Text = "11:30", Value = DepartureTime.C1130.ToString()},
                     new {Text = "13:30", Value = DepartureTime.C1330.ToString()},
                     new {Text = "15:00", Value = DepartureTime.C1500.ToString()}
                 };
            var sgList = new List<object>
                 {
                     new {Text = "7:00", Value = DepartureTime.C700.ToString()},
                     new {Text = "8:30", Value = DepartureTime.C830.ToString()},
                     new {Text = "10:00", Value = DepartureTime.C1000.ToString()},
                     new {Text = "11:30", Value = DepartureTime.C1130.ToString()},
                     new {Text = "13:00", Value = DepartureTime.C1300.ToString()},
                     new {Text = "14:30", Value = DepartureTime.C1430.ToString()},
                     new {Text = "16:00", Value = DepartureTime.C1600.ToString()},
                     new {Text = "17:30", Value = DepartureTime.C1730.ToString()}
                 };
            return Json(tripName == TripName.MA ? maList : sgList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrUpdatePassenger(PassengerModel passengerModel, TripModel tripModel)
        {
            var passenger = Mapper.Map<PassengerModel, Passenger>(passengerModel);
            var trip = tripService.AddOrUpdateTripFollowDepartureInfo(Mapper.Map<TripModel, Trip>(tripModel));
            passenger.Trip = trip;
            passenger.TripId = trip.Id;
            passengerService.AddOrUpdatePassenger(passenger);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePassenger(PassengerModel passengerModel, TripModel tripModel)
        {
                var passenger = passengerService.GetPassenger(passengerModel.Id);
                passengerService.DeletePassenger(passenger);
                return Json(passengerModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItem(string models, ItemSearchModel searchModel)
        {
            var parameter = JsonConvert.DeserializeObject<ItemSearchModel>(models);
            parameter.fromDate = searchModel.fromDate;
            parameter.toDate = searchModel.toDate;
            parameter.fromTime = searchModel.fromTime;
            parameter.toTime = searchModel.toTime;
            parameter.TripName = searchModel.TripName;
            parameter.noPaging = searchModel.noPaging;
            var result = itemService.GetItems(parameter);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrUpdateItem(string models, TripModel tripModel)
        {
            var items = JsonConvert.DeserializeObject<List<ItemModel>>(models);
            foreach (var itemModel in items)
            {
                var item = Mapper.Map<ItemModel, Item>(itemModel);
                var trip = tripService.AddOrUpdateTripFollowDepartureInfo(Mapper.Map<TripModel, Trip>(tripModel));
                item.Trip = trip;
                item.TripId = trip.Id;
                itemService.AddOrUpdateItem(item);
                itemModel.Id = item.Id;
                itemModel.TripDepartureTime = item.Trip.DepartureTime.ToString();
            }
            return Json(new { Data = items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteItem(string models, TripModel tripModel)
        {
            var items = JsonConvert.DeserializeObject<List<ItemModel>>(models);
            foreach (var itemModel in items)
            {
                var item = itemService.GetItem(itemModel.Id);
                itemService.DeleteItem(item);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}