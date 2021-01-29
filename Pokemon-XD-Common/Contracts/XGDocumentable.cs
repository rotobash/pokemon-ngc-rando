using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Contracts
{
    public interface IXGDocumentable
    {
        static string DocumentableClassName {get;}
	    string DocumentableName {get;}
	    bool IsDocumentable { get; }
	    static List<string> DocumentableKeys { get; }
        string DocumentableValue(string key);
    }

    public abstract class XGDocumentable : IXGDocumentable
    {
        public string DocumentableName { get; }
		public bool IsDocumentable { get; } = true;
        public static List<string> DocumentableKeys { get; }

        public abstract string DocumentableValue(string key);
	
		public string DocumentableFields() {
			var text = "";
			foreach(var key in DocumentableKeys) {
				text += $"\n{key}: {DocumentableValue(key)}";
			}
			return text;
		}
	
		public string DocumentableData() {
			return $"{DocumentableName}\n" + DocumentableFields();
		}
	
		//public void SaveDocumentedData() {
		//	var documentationFolder = XGFolders.NameAndFolder("Documentation", .Reference);
		//	var folder = XGFolders.NameAndFolder(DocumentableClassName, documentationFolder);
		//	folder.CreateDirectory();

		//	var file = XGFiles.NameAndFolder(DocumentableName + ".txt", folder);
		//	XGUtility.SaveString(DocumentableData(), file);
		//}
    }
}
