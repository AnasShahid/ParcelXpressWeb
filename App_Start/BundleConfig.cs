using System.Web;
using System.Web.Optimization;

namespace ParcelXpress
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region other bundles
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Lib/angular/angular.js",
                "~/Lib/angular/angular-route.js"
                ));


       
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

           

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
            #endregion

            # region my bundles

            bundles.Add(new ScriptBundle("~/bundles/scripts/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.unobtrusive*",
                 "~/Scripts/jquery.validate*"
                 

                ));

            bundles.Add(new ScriptBundle("~/bundles/scripts/libraries").Include(
               
                "~/Lib/bootstrap/js/bootstrap.js",
                "~/Lib/toastr/toastr.js",
                "~/Lib/bootstrap-dialog/bootstrap-dialog.js",
                "~/Lib/bootstrap-datepicker/bootstrap-datepicker.js",
                "~/Lib/material-design-lite/material.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/scripts/myscripts").Include(
                "~/MyScripts/jquery/ajax-methods.js",
                 "~/MyScripts/jquery/ajax-method-without-form.js",
                 "~/MyScripts/jquery/custom-scripts.js"
                ));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
               //"~/Lib/bootstrap/css/bootstrap.css",
               "~/Lib/toastr/toastr.css",
               "~/Styles/custom.css",
               "~/Styles/AdminLTE.css",
               "~/Content/PagedList.css",
               "~/Lib/bootstrap-dialog/bootstrap-dialog.css",
               "~/Lib/bootstrap-datepicker/bootstrap-datepicker.css"
               ));
            #endregion
        }
    }
}