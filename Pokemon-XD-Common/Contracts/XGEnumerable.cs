using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDCommon.Contracts
{
    public interface IXGEnumerable
    {
        string EnumerableName { get; }
        string EnumerableValue { get; }
        string EnumerableClassName { get; }
        static List<IXGEnumerable> AllValues { get; }
    }

    public abstract class XGEnumerable: IXGEnumerable
    {
        public virtual string EnumerableName { get; }

        public virtual string EnumerableValue { get; }
		public virtual string EnumerableClassName { get; }
		public static List<IXGEnumerable> AllValues { get; }

		static void DocumentEnumerationData()
		{
			//var folder = XGFolders.NameAndFolder("Enumerations", .Reference);
			//folder.CreateDirectory();
			//var file = XGFiles.NameAndFolder(EnumerableClassName + ".txt", folder);

			//var text = $"{EnumerableClassName} - count: {AllValues.Count}\n";
			//foreach(var value in AllValues) {
			//	text += $"\n{value.EnumerableName}";
			//}

			//XGUtility.saveString(text, file);
		}

		//public static XGFolders EncodedDataFolder() {
		//	var encodingFolder = XGFolders.nameAndFolder("Raw Data", .Reference);
		//	return XGFolders.nameAndFolder(EnumerableClassName, encodingFolder);
		//}
	
		//public static IEnumerable<XGFiles> EncodedJSONFiles() {
		//	return EncodedDataFolder().files.Where(f => f.fileType == ".json");
		//}
	
		//public static IEnumerable<Encodable> EncodableValues() {
		//	return (AllValues as IEnumerable<Encodable>) ?? Enumerable.Empty<Encodable>();
		//}
	
		//public static void EncodeData(Func<Encodable, string> filename) {
		//	var folder = EncodedDataFolder();
		//	folder.CreateDirectory();
		
		//	foreach(var value in EncodableValues()) {
		//		var file = XGFiles.nameAndFolder(filename(value) + ".json", folder);
		//		value.WriteJSON(to: file);
		//	}
		//}
	
		//public static IEnumerable<XGFiles> EncodedData() {
		//	if(!EncodedDataFolder().exists) { return Enumerable.Empty<XGFiles>(); }

		//	return EncodedJSONFiles();
		//}
	}
}
