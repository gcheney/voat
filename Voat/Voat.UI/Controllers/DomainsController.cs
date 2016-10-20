﻿/*
This source file is subject to version 3 of the GPL license,
that is bundled with this package in the file LICENSE, and is
available online at http://www.gnu.org/licenses/gpl.txt;
you may not use this file except in compliance with the License.

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

All portions of the code written by Voat are Copyright (c) 2015 Voat, Inc.
All Rights Reserved.
*/

using System;
using System.Linq;
using System.Web.Mvc;
using Voat.Data.Models;
using Voat.Utilities;

namespace Voat.Controllers
{
    public class DomainsController : BaseController
    {
        private readonly voatEntities _db = new voatEntities();

        //TODO: This needs caching
        public ActionResult Index(int? page, string domainname, string sortingmode)
        {
            const int pageSize = 25;
            int pageNumber = (page ?? 0);

            if (pageNumber < 0 || String.IsNullOrWhiteSpace(domainname))
            {
                return View("~/Views/Error/404.cshtml");
            }

            ViewBag.SelectedSubverse = "domains";
            ViewBag.SelectedDomain = domainname;

            domainname = domainname.Trim().ToLower();

            //restrict disabled subs from result list
            IQueryable<Submission> submissions = (from m in _db.Submissions
                                                  join s in _db.Subverses on m.Subverse equals s.Name
                                                  where
                                                  !s.IsAdminDisabled.Value
                                                  && !m.IsDeleted
                                                  && m.Type == 2
                                                  && m.Url.ToLower().Contains(domainname)
                                                  select m);

            if (sortingmode == "new")
            {
                ViewBag.SortingMode = sortingmode;
                submissions = submissions.OrderByDescending(x => x.CreationDate);
            }
            else
            {
                ViewBag.SortingMode = "hot";
                submissions = submissions.OrderByDescending(x => x.Rank).ThenByDescending(x => x.CreationDate);
            }

            var paginatedSubmissions = new PaginatedList<Submission>(submissions, page ?? 0, pageSize);

            ViewBag.Title = "Showing all submissions which link to " + domainname;
            return View("Index", paginatedSubmissions);
        }

        //public ActionResult @New(int? page, string domainname, string sortingmode)
        //{
        //    //sortingmode: new, contraversial, hot, etc
        //    ViewBag.SortingMode = sortingmode;

        //    if (!sortingmode.Equals("new")) return RedirectToAction("Index", "Home");
        //    ViewBag.SelectedSubverse = "domains";
        //    ViewBag.SelectedDomain = domainname + "." + ext;

        //    const int pageSize = 25;
        //    int pageNumber = (page ?? 0);

        //    if (pageNumber < 0)
        //    {
        //        return View("~/Views/Error/404.cshtml");
        //    }

        //    //check if at least one submission for given domain was found, if not, send to a page not found error
        //    //IQueryable<Message> submissions = _db.Messages
        //    //    .Where(x => x.Name != "deleted" & x.Type == 2 & x.MessageContent.ToLower().Contains(domainname + "." + ext))
        //    //    .OrderByDescending(s => s.Date);

        //    IQueryable<Submission> submissions = (from m in _db.Submissions
        //                                          join s in _db.Subverses on m.Subverse equals s.Name
        //                                       where !s.IsAdminDisabled.Value && !m.IsDeleted & m.Type == 2 & m.Content.ToLower().Contains(domainname + "." + ext)
        //                                       orderby m.CreationDate descending
        //                                       select m);

        //    var paginatedSubmissions = new PaginatedList<Submission>(submissions, page ?? 0, pageSize);

        //    ViewBag.Title = "Showing all newest submissions which link to " + domainname;
        //    return View("Index", paginatedSubmissions);
        //}
    }
}
