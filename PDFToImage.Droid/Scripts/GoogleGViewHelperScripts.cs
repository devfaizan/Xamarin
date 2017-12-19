using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PDFToImage.Droid.Scripts
{
    public static class GoogleGViewHelperScripts
    {
        public static readonly string ScriptGViewLoaded = "document.querySelectorAll('[role=\"document\"]')[0].clientHeight != 0";

        public static readonly string ScriptGetClientHeight = "(parseInt((document.querySelectorAll('[role=\"document\"]')[0].clientHeight))).toString()";

        public static readonly string ScriptRemoveBar = @"(function(){ 
                                            try{
                                                var tooltip = document.querySelectorAll('[role=""toolbar""]');
                                                if (tooltip.length > 0) {tooltip[0].remove();}
                                                var parent = document.querySelectorAll('[role=""button""]')[0].parentElement;
                                                var childs = parent.children;   
                                                var childCount = childs.length;
                                                if(childs.length > 0){
                                                    for (i = 0; i < childCount - 1; i++) { 
                                                        childs = parent.children;   
                                                        childs[0].remove();
                                                    }
                                                    return true;
                                                }
                                                else{
                                                    return true;
                                                }
                                            }
                                            catch(err)
                                            { return err; }
                                          })()";

        public static readonly string ScriptGetInnerHTML = "document.querySelectorAll('[role=\"document\"]')[0].innerHTML.toString()";
        public static readonly string ScriptScrollBottom = @"(function(){for (i = 0; i < document.querySelectorAll('[role=""document""]')[0].children.length; i++) { 
                                       document.querySelectorAll('[role=""document""]')[0].parentElement.scrollTop 
                                        = document.querySelectorAll('[role=""document""]')[0].children[i].offsetTop } return false})() ";
        public static readonly string ScriptCheckPDFRendered = @"(function(){ 
                                            var childs = document.querySelectorAll('[role=""document""]')[0].children;   
                                            if(childs.length > 0){
                                                for (i = 0; i < childs.length; i++) { 
                                                    if(childs[i].querySelectorAll('img')[0] == undefined){ return false; }
                                                }
                                                return true;
                                            }
                                            else{
                                                return true;
                                            }
                                          })()";
    }
}