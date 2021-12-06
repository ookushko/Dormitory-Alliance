﻿using DormitoryAlliance.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DormitoryAlliance.Client.Controllers
{
    [Authorize(Roles = "admin")]
    public class ManageController : Controller
    {
        private DormitoryAllianceDbContext _context;

        public ManageController(DormitoryAllianceDbContext context)
        {
            _context = context;
        }

        // GET: Manage
        public ActionResult StudentIndex()
        {
            var students = _context.Students
                .Include(s => s.Group)
                .Include(s => s.Room).Take(300).ToList();

            return View(students);
        }

        // GET: Manage/StudentDetails/5
        public ActionResult StudentDetails(int id)
        {
            var student = _context.Students
                .Include(s => s.Group)
                .Include(s => s.Room)
                .FirstOrDefault(x => x.Id == id);

            return View(student);
        }

        // GET: Manage/StudentCreate
        public ActionResult StudentCreate()
        {
            var rooms = _context.Rooms
                        .Include(room => room.Dormitory)
                        .OrderBy(room => room.Number)
                        .Select(room => new
                        {
                            room.Id,
                            Name = $"{(room.Number % 100 == 27 ? (room.Number.ToString()[0] + "12A") : room.Number)} ({room.DormitoryId})"
                        });

            ViewBag.Rooms = new SelectList(rooms, "Id", "Name");

            var groups = _context.Groups
                .OrderBy(group => group.Name);

            ViewBag.Groups = new SelectList(groups, "Id", "Name");

            return View();
        }

        // POST: Manage/StudentCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentCreate(IFormCollection collection)
        {
            try
            {
                var student = new Student
                {
                    Name = collection["Name"],
                    Surname = collection["Surname"],
                    Patronymic = collection["Patronymic"],
                    RoomId = int.TryParse(collection["RoomId"], out int rid) ? rid : 0,
                    GroupId = int.TryParse(collection["GroupId"], out int gid) ? gid : 0,
                    Course = int.TryParse(collection["Course"], out int course) ? course : 0
                };

                _context.Students.Add(student);

                _context.SaveChanges();

                return RedirectToAction(nameof(StudentIndex));
            }
            catch
            {
                return View();
            }
        }

        // GET: Manage/StudentEdit/5
        public ActionResult StudentEdit(int? id)
        {
            if (id != null)
            {
                var student = _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Room)
                    .FirstOrDefault(x => x.Id == id);

                if (student?.Name != null)
                {
                    var rooms = _context.Rooms
                        .Include(room => room.Dormitory)
                        .OrderBy(room => room.Number)
                        .Select(room => new
                        {
                            room.Id,
                            Name = $"{(room.Number % 100 == 27 ? (room.Number.ToString()[0] + "12A" ) : room.Number)} ({room.DormitoryId})"
                        });

                    ViewBag.Rooms = new SelectList(rooms, "Id", "Name");

                    var groups = _context.Groups
                        .OrderBy(group => group.Name);

                    ViewBag.Groups = new SelectList(groups, "Id", "Name");

                    return View(student);
                }
            }

            Console.WriteLine("Incorect id");
            return RedirectToAction(nameof(StudentIndex));
        }

        // POST: Manage/StudentEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentEdit(int id, IFormCollection collection)
        {
            try
            {
                var student = new Student
                {
                    Id = id,
                    Name = collection["Name"],
                    Surname = collection["Surname"],
                    Patronymic = collection["Patronymic"],
                    RoomId = int.TryParse(collection["RoomId"], out int rid) ? rid : 0,
                    GroupId = int.TryParse(collection["GroupId"], out int gid) ? gid : 0,
                    Course = int.TryParse(collection["Course"], out int course) ? course : 0
                };

                _context.Students.Update(student);

                _context.SaveChanges();

                return RedirectToAction(nameof(StudentIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        // GET: Manage/StudentDelete/5
        public ActionResult StudentDelete(int id)
        {
            var student = _context.Students.Where(s => s.Id == id)
                .Include(x => x.Group)
                .Include(x => x.Room).ThenInclude(x => x.Dormitory)
                .FirstOrDefault();

            if (student == null)
            {
                return RedirectToAction(nameof(StudentIndex));
            }

            return View(student);
        }

        // POST: Manage/StudentDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentDelete(int id, IFormCollection collection)
        {
            try
            {
                var student = _context.Students.Find(id);

                if (student != null)
                {
                    _context.Students.Remove(student);

                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(StudentIndex));
            }
            catch
            {
                return View();
            }
        }
    }
}
