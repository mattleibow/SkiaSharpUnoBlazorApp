using Newtonsoft.Json;

namespace ClassLibrary
{
	public class RedrawData
	{
		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("color")]
		public string Color { get; set; }

		[JsonProperty("size")]
		public int Size { get; set; }

		public override string ToString() =>
			$"{{Text={Text}, Color={Color}, Size={Size}}}";

		//

		public static RedrawData FromJson(string json) =>
			JsonConvert.DeserializeObject<RedrawData>(json);

		public string ToJson() =>
			JsonConvert.SerializeObject(this);
	}
}
