
using System.IO;
using Newtonsoft.Json;

namespace Game
{
	public static class SaveManager
	{
		public static void Save(string path, object data)
		{
			var json = JsonConvert.SerializeObject(data);

			File.WriteAllText(path, json);
		}

		public static T Load<T>(string path) where T: new()
		{
			var json = File.ReadAllText(path);

			var data = JsonConvert.DeserializeObject<T>(json);

			return data;
		}
	}
}