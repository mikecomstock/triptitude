//        public ActionResult Tags()
//        {
//            ViewBag.Tags = new TagsRepo().FindAll();
//            return View();
//        }

//        [HttpGet]
//        public ActionResult Tag(int id)
//        {
//            ViewBag.Tag = new TagsRepo().Find(id);
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Tag(int id, FormCollection form)
//        {
//            var tagsRepo = new TagsRepo();
//            Tag tag = tagsRepo.Find(id);
//            tag.Name = form["tag.Name"];
//            tagsRepo.Save();

//            return Redirect("/admin/tags");
//        }

//        public ActionResult Items()
//        {
//            ViewBag.Items = new ItemsRepo().FindAll();
//            return View();
//        }

//        [HttpGet]
//        public ActionResult Item(int id)
//        {
//            ViewBag.Item = new ItemsRepo().Find(id);
//            ViewBag.AllTags = new TagsRepo().FindAll();
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Item(int id, FormCollection form)
//        {
//            var itemsRepo = new ItemsRepo();
//            Item item = itemsRepo.Find(id);
//            item.Name = form["item.Name"];
//            item.URL = form["item.URL"];

//            item.Tags.Clear();
//            if (form["item.tags"] != null)
//            {
//                var tagsRepo = new TagsRepo();
//                var tagIds = form["item.Tags"].Split(',').Select(i => int.Parse(i));
//                var newTags = tagsRepo.FindAll().Where(t => tagIds.Contains(t.Id));
//                item.Tags = newTags.ToList();
//            }

//            itemsRepo.Save();

//            return Redirect("/admin/items");
//        }
//    }
//}