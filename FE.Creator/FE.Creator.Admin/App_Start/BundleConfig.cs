﻿using System.Web;
using System.Web.Optimization;

namespace FE.Creator.Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/base/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                        "~/Content/base/respond.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //adminlte
            bundles.Add(new StyleBundle("~/css/adminlte").Include(
                   "~/Content/adminlte-2.3.6/bootstrap/css/bootstrap.min.css",
                   "~/Content/adminlte-2.3.6/plugins/iCheck/flat/blue.css",
                   "~/Content/adminlte-2.3.6/plugins/morris/morris.css",
                   "~/Content/adminlte-2.3.6/plugins/jvectormap/jquery-jvectormap-1.2.2.css",
                   "~/Content/adminlte-2.3.6/plugins/datepicker/datepicker3.css",
                   "~/Content/adminlte-2.3.6/plugins/daterangepicker/daterangepicker.css",
                   "~/Content/adminlte-2.3.6/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
                   "~/Content/adminlte-2.3.6/plugins/pace/pace.min.css",
                   "~/Content/adminlte-2.3.6/plugins/ui-notification/angular-ui-notification.min.css",
                   "~/Content/adminlte-2.3.6/plugins/iCheck/all.css",
                   "~/Content/adminlte-2.3.6/plugins/ui-select-master/select.min.css",
                   "~/Content/adminlte-2.3.6/plugins/angularjs-slider/angular-slider.min.css",
                    "~/Content/adminlte-2.3.6/dist/css/AdminLTE.min.css",
                   "~/Content/adminlte-2.3.6/dist/css/skins/_all-skins.min.css",
                   "~/Content/apps/css/angular-rating.css",
                    "~/Content/adminlte-2.3.6/plugins/ckeditor/plugins/codesnippet/lib/highlight/styles/vs.css",
                   "~/Content/apps/css/app.css"
               ));
            bundles.Add(new StyleBundle("~/css/bootstrap-weather").Include(
                "~/Content/adminlte-2.3.6/bootstrap/css/weather-icons.min.css",
                "~/Content/adminlte-2.3.6/bootstrap/css/weather-icons-wind.min.css"
                ));
            bundles.Add(new ScriptBundle("~/js/adminlte").Include(
                    "~/Content/adminlte-2.3.6/plugins/jQuery/jquery-2.2.3.min.js",
                    "~/Content/adminlte-2.3.6/bootstrap/js/bootstrap.min.js",
                    "~/Content/adminlte-2.3.6/plugins/morris/morris.min.js",
                    "~/Content/adminlte-2.3.6/plugins/sparkline/jquery.sparkline.min.js",
                    "~/Content/adminlte-2.3.6/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                    "~/Content/adminlte-2.3.6/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                    "~/Content/adminlte-2.3.6/plugins/knob/jquery.knob.js",
                    "~/Content/adminlte-2.3.6/plugins/moment/moment-with-locales.min.js",
                     "~/Content/adminlte-2.3.6/plugins/datepicker/bootstrap-datepicker.js",
                    "~/Content/adminlte-2.3.6/plugins/daterangepicker/daterangepicker.js",
                    "~/Content/adminlte-2.3.6/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                    "~/Content/adminlte-2.3.6/plugins/slimScroll/jquery.slimscroll.min.js",
                    "~/Content/adminlte-2.3.6/plugins/fastclick/fastclick.js",
                    "~/Content/adminlte-2.3.6/plugins/pace/pace.min.js",
                    "~/Content/adminlte-2.3.6/plugins/iCheck/icheck.min.js",
                    "~/Content/adminlte-2.3.6/dist/js/app.js"
                ));

            bundles.Add(new ScriptBundle("~/js/ckeditor").Include(
                    "~/Content/adminlte-2.3.6/plugins/ckeditor/ckeditor.js" 
                ));

            bundles.Add(new ScriptBundle("~/js/echarts").Include(
                    "~/Content/adminlte-2.3.6/plugins/echarts/echarts.min.js"
                ));

            bundles.Add(new ScriptBundle("~/js/ckeditor-jquery").Include(
                   "~/Content/adminlte-2.3.6/plugins/ckeditor/adapters/jquery.js"
              ));
           
             bundles.Add(new ScriptBundle("~/js/highlight").Include(
                    "~/Content/adminlte-2.3.6/plugins/ckeditor/plugins/codesnippet/lib/highlight/highlight.pack.js"
                ));
            bundles.Add(new ScriptBundle("~/js/apputilities").Include(
                    "~/Content/apps/apputilities.js"
                ));
            bundles.Add(new ScriptBundle("~/js/angularjs").Include(
                "~/Content/angularjs/angular.min.js",
                "~/Content/angularjs/angular-sanitize.min.js"
             ));
            bundles.Add(new ScriptBundle("~/js/angularroute").Include(
               "~/Content/angularjs/angular-route.min.js"
            ));
            bundles.Add(new ScriptBundle("~/js/angularmessage").Include(
              "~/Content/angularjs/angular-messages.min.js",
              "~/Content/adminlte-2.3.6/plugins/ui-notification/angular-ui-notification.min.js"
            ));
            bundles.Add(new ScriptBundle("~/js/angularselect").Include(
             "~/Content/adminlte-2.3.6/plugins/ui-select-master/select.min.js"
          ));
            bundles.Add(new ScriptBundle("~/js/angularupload").Include(
           "~/Content/adminlte-2.3.6/plugins/ng-file-upload-master/ng-file-upload-shim.min.js",
           "~/Content/adminlte-2.3.6/plugins/ng-file-upload-master/ng-file-upload.min.js"
            ));

            bundles.Add(new ScriptBundle("~/js/adminapp/objectrepository").Include(
                  "~/Content/apps/objectrepository.js"
                ));
            bundles.Add(new ScriptBundle("~/js/adminapp/dataservice").Include(
                  "~/Content/apps/dataservice.js"
            ));
            bundles.Add(new ScriptBundle("~/js/adminapp/pageservice").Include(
                  "~/Content/apps/pageservice.js"
            ));
            bundles.Add(new ScriptBundle("~/js/adminapp/objectutilservice").Include(
                "~/Content/apps/objectutilservice.js"
            ));
            bundles.Add(new ScriptBundle("~/js/adminapp/usersimpleprofile").Include(
                     "~/Content/apps/usersimpleprofile.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/definitiongroup").Include(
                     "~/Content/apps/definitiongroupcontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/objectdefinition").Include(
                    "~/Content/apps/objectdefinitioncontroller.js"
               ));

            bundles.Add(new ScriptBundle("~/js/adminapp/generalobject").Include(
                "~/Content/apps/generalobjectcontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/sysusers").Include(
                "~/Content/apps/usercontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/licensemgr").Include(
                 "~/Content/apps/licensecontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/userprofile").Include(
                    "~/Content/apps/userprofilecontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/document").Include(
                    "~/Content/apps/documentcontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/book").Include(
                    "~/Content/apps/bookscontroller.js"
               ));

            bundles.Add(new ScriptBundle("~/js/adminapp/systemevent").Include(
                "~/Content/apps/systemevent.js"));

            bundles.Add(new ScriptBundle("~/js/adminapp/images").Include(
                    "~/Content/apps/imagescontroller.js"
                ));
            bundles.Add(new ScriptBundle("~/js/adminapp/task").Include(
                    "~/Content/apps/taskcontroller.js"
                ));
            bundles.Add(new ScriptBundle("~/js/adminapp/contacts").Include(
                    "~/Content/apps/contactcontroller.js"
                ));
            bundles.Add(new ScriptBundle("~/js/adminapp/diary").Include(
                   "~/Content/apps/diarycontroller.js"
               ));
            bundles.Add(new ScriptBundle("~/js/adminapp/article").Include(
                   "~/Content/apps/articlecontroller.js"
               ));
            bundles.Add(new ScriptBundle("~/js/adminapp/accountrecord").Include(
                   "~/Content/apps/accountrecordscontroller.js"
               ));
            bundles.Add(new ScriptBundle("~/js/adminapp/dashboard").Include(
                    "~/Content/apps/dashboardcontroller.js"
                ));
            bundles.Add(new ScriptBundle("~/js/adminapp/appsettings").Include(
                    "~/Content/apps/appsettingscontroller.js"
                ));
        }
    }
}
