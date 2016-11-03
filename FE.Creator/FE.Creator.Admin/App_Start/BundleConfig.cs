using System.Web;
using System.Web.Optimization;

namespace FE.Creator.Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //adminlte
            bundles.Add(new StyleBundle("~/css/adminlte").Include(
                   "~/Content/adminlte-2.3.6/bootstrap/css/bootstrap.min.css",
                   "~/Content/adminlte-2.3.6/plugins/iCheck/flat/blue.css",
                   "~/Content/adminlte-2.3.6/plugins/morris/morris.css",
                   "~/Content/adminlte-2.3.6/plugins/jvectormap/jquery-jvectormap-1.2.2.css",
                   "~/Content/adminlte-2.3.6/plugins/datepicker/datepicker3.css",
                   "~/Content/adminlte-2.3.6/plugins/daterangepicker/daterangepicker.css",
                   "~/Content/adminlte-2.3.6/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
                   "~/Content/adminlte-2.3.6/dist/css/AdminLTE.min.css",
                   "~/Content/adminlte-2.3.6/dist/css/skins/_all-skins.min.css"
                   
               ));

            bundles.Add(new ScriptBundle("~/js/adminlte").Include(
                    "~/Content/adminlte-2.3.6/bootstrap/js/bootstrap.min.js",
                    "~/Content/adminlte-2.3.6/dist/js/app.min.js",
                    "~/Content/adminlte-2.3.6/plugins/jQuery/jquery-2.2.3.min.js",
                    "~/Content/adminlte-2.3.6/bootstrap/js/bootstrap.min.js",
                    "~/Content/adminlte-2.3.6/plugins/morris/morris.min.js",
                    "~/Content/adminlte-2.3.6/plugins/sparkline/jquery.sparkline.min.js",
                    "~/Content/adminlte-2.3.6/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                    "~/Content/adminlte-2.3.6/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                    "~/Content/adminlte-2.3.6/plugins/knob/jquery.knob.js",
                    "~/Content/adminlte-2.3.6/plugins/daterangepicker/daterangepicker.js",
                    "~/Content/adminlte-2.3.6/plugins/datepicker/bootstrap-datepicker.js",
                    "~/Content/adminlte-2.3.6/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                    "~/Content/adminlte-2.3.6/plugins/slimScroll/jquery.slimscroll.min.js",
                    "~/Content/adminlte-2.3.6/plugins/fastclick/fastclick.js",
                    "~/Content/adminlte-2.3.6/dist/js/app.min.js"
                ));

            bundles.Add(new ScriptBundle("~/js/angularjs").Include(
                "~/Content/angularjs/angular.min.js",
                "~/Content/angularjs/angular-route.min.js"
             ));

            bundles.Add(new ScriptBundle("~/js/adminapp").Include(
                  "~/Content/apps/objectrepository.js",
                  "~/Content/apps/dataservice.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/definitiongroup").Include(
                     "~/Content/apps/definitiongroupcontroller.js"
                ));

            bundles.Add(new ScriptBundle("~/js/adminapp/objectdefinition").Include(
                    "~/Content/apps/objectdefinitioncontroller.js"
               ));

        }
    }
}
