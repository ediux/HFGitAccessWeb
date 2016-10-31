using GitTools;
using HFGitAccessWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HFGitAccessWeb.Controllers
{
    public class RepositoryController : Controller
    {
        string baseFolder = ConfigurationManager.AppSettings["GitBaseFolder"];
        GitDataSource db = new GitDataSource();
        // GET: Repository
        public Task<ActionResult> Index()
        {
            return Task.Run<ActionResult>(() =>
            {
                var gitExePath = ConfigurationManager.AppSettings["GitExePath"];
                var gitRootFolder = ConfigurationManager.AppSettings["GitBaseFolder"];
                var msg = "";
                if (!System.IO.File.Exists(gitExePath))
                {
                    msg = gitExePath + " does not exist. ";
                }
                if (!Directory.Exists(gitRootFolder))
                {
                    msg += gitRootFolder + " does not exist. ";
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    TempData.Add("ErrorMsg", msg);

                }

                var model = BindData();

                return View(model);
            });
        }

        // GET: Repository/Details/5
        // 查看變更紀錄
        public ActionResult Details(string id)
        {           
            var model = db.RepositoryGraph.FirstOrDefault(w => w.Id == id || w.Id ==  id+".");
            if (model == null)
            {
                model = db.RepositoryGraph.FirstOrDefault(w => w.Name == id);

                if (model == null)
                {
                    TempData.Add("ErrorMsg", "無此儲存庫!");
                    model = new Graph(new Repository() { Id = id, Name = id });
                }
            }          
            return View(model);
        }

        // 建立遠端共用儲存庫
        // GET: Repository/Create
        public ActionResult Create()
        {
            return View(new CreateRepositoryViewModel());
        }

        // POST: Repository/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "")]CreateRepositoryViewModel createReposirotyViewModel)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var folder = createReposirotyViewModel.CreateFolderName; // tbCreateFolderName.Text;

                    if (string.IsNullOrEmpty(folder)) return View();

                    string ext = Path.GetExtension(folder);
                    if (string.IsNullOrEmpty(ext) || ext != Git.GIT_EXTENSION)
                    {
                        folder = Path.ChangeExtension(folder, Git.GIT_EXTENSION);
                    }

                    folder = folder.Replace(" ", "-");
                    if (Directory.Exists(folder)) return View();


                    var gitBaseDir = ConfigurationManager.AppSettings["GitBaseFolder"];
                    Git.Run("init --bare " + folder, gitBaseDir);
                    //BindData();

                    return RedirectToAction("Index");
                }

                return View(createReposirotyViewModel);
            }
            catch
            {
                return View();
            }
        }

        //// GET: Repository/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Repository/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Repository/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Repository/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        #region Helper Function
        private IEnumerable<GitTools.Repository> BindData()
        {
            //      .Where(w => w.Parent.Name.EndsWith("."+Git.GIT_EXTENSION))
            var host = Request.Url.Host;
            var allrepos = db.Repositories.ToList();
            foreach (var item in allrepos)
            {
                string id = item.Id;
                id = id.TrimEnd('.');
                item.Id = id;

                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = id;
                }
                item.AccessUrl = string.Format("{0}://{1}:{3}{2}", Request.Url.Scheme, host, item.AccessUrl.Replace("/.git",""), Request.Url.Port);

            }
            return allrepos.AsEnumerable();
            //var directoryInfo = new DirectoryInfo(baseFolder);
            //var folders = directoryInfo.EnumerateDirectories(string.Format("*.{0}", Git.GIT_EXTENSION), SearchOption.AllDirectories)          
            //    .Where(w=>w.Name.EndsWith("."+Git.GIT_EXTENSION) && w.Name.Replace("."+Git.GIT_EXTENSION,"").Length>0)
            //    .Select(d => new RepositoryIndexViewModel()
            //    {
            //        Name = d.Name.Replace("." + Git.GIT_EXTENSION, ""),
            //        Id = d.FullName.Substring(baseFolder.Length + 1)
            //                                              .Replace("\\", ".")
            //                                              .Replace("." + Git.GIT_EXTENSION, "")
            //    }).ToList();

            //foreach (var item in folders)
            //{
            //    item.URL = GetUrl(item.Id);
            //}

            //return folders;
        }

        protected string GetUrl(string Id)
        {
            //Url.Action("Index", "Repository", new { id = d.FullName })
            var host = Request.Url.Host;

            var directory = Id;
            directory = directory.Replace("\\", "/");

            var port = Request.Url.Port;

            return string.Format("{0}://{1}:{3}/{2}." + Git.GIT_EXTENSION, Request.Url.Scheme, host, directory, Request.Url.Port);
        }
        #endregion
    }
}
