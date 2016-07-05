using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Web.Helpers;
using System.IO;
using System.Data.Objects.SqlClient;
using System.Diagnostics;
using QuizWeb.Data;
using QuizWeb.Misc;
using QuizWeb.Models;
using Resources;
using PagedList;

namespace QuizWeb.Controllers
{
    [Authorize(Roles="Administrator")]
    public class UserController : BaseController
    {


        public UserController(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        //
        // GET: /User/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";


            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;

            var users = from s in unitOfWork.UsersRepository.Get()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Login.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    users = users.OrderByDescending(s => s.Login);
                    break;
                default:
                    users = users.OrderBy(s => s.Login);
                    break;
            }

            int pageSize = Constants.ITEMS_PER_PAGE;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /User/Details/5
        public ViewResult Details(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            User user = unitOfWork.UsersRepository.GetByID(id);
            return View(user);
        }

        //
        // GET: /User/Create
        public ActionResult Create()
        {
            User user = new User();
            return View(user);
        }

        //
        // POST: /User/Create
        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                    unitOfWork.UsersRepository.Add(user);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", Resource.UnableToSave);
            }

            return View(user);
        }

       
        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            User user = unitOfWork.UsersRepository.GetByID(id);
            return View(user);
        }

        //
        // POST: /User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            var userToUpdate = unitOfWork.UsersRepository.GetByID(id);
            if (TryUpdateModel(userToUpdate, ""))
            {
                try
                {
                  
                    unitOfWork.UsersRepository.Update(userToUpdate);
                    unitOfWork.Save();

                    return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page });
                }
                catch (DataException)
                {
                    //Log the error (add a variable name after DataException)
                    ModelState.AddModelError("", Resource.UnableToSave);
                }
            }
       
            return View(userToUpdate);
        }

        //
        // GET: /User/Delete/5
        public ActionResult Delete(int id, bool? deleteError, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.Page = page;
            if (deleteError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = Resource.UnableToSave;
            }
            User user = unitOfWork.UsersRepository.GetByID(id);
            return View(user);
        }


        //
        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string sortOrder, string currentFilter, int? page)
        {

            try
            {
                User user = unitOfWork.UsersRepository.GetByID(id);
         
                unitOfWork.UsersRepository.Delete(id);

                unitOfWork.Save();

                return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page=page });
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                return RedirectToAction("Delete",
                    new System.Web.Routing.RouteValueDictionary { 
                { "id", id }, 
                { "deleteError", true } });
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}