using System.Reflection;
using System.Runtime;

namespace Rotation.Preset;

public static class PresetReader {
	public static Setting? LoadPreset(string pPath, Setting pDefault) {
		var type = typeof(Setting);
		var constructor = type.GetConstructors().First();
		var parameter = constructor.GetParameters();
		var dict = parameter.ToDictionary(p => p.Name, p => {
			var v = type.GetProperty(p.Name).GetValue(pDefault);
			return (p.ParameterType, Value: v);
		});
		
		if (!pPath.Contains('.')) pPath += ".preset";
		pPath = Path.Combine("Preset", pPath);
		var parseMethodArg = new Type[] { typeof(string) };
		foreach (var line in File.ReadAllLines(pPath)) {
			if(string.IsNullOrWhiteSpace(line)) continue;
			var splitPoint = line.IndexOf(':');
			if(splitPoint == -1) continue;
			var (name, value) = (line[..splitPoint].Trim(), line[(splitPoint + 1)..].Trim());
			if (dict.ContainsKey(name)) {
				var t = dict[name].ParameterType;
				if (t == typeof(string)) {
					dict[name] = dict[name] with { Value = value };
					continue;
				}
				if (t.IsEnum) {
					dict[name] = dict[name] with { Value = Enum.Parse(t, value)};
					continue;
				}
				var parseMethod = t.GetMethod("Parse", parseMethodArg);
				var v = parseMethod.Invoke(null, [value]);
				dict[name] = dict[name] with{ Value = v};
			}
		}
		var setting = constructor.Invoke(parameter.Select(p => dict[p.Name].Value).ToArray()) as Setting;
		setting?.Camera = pDefault.Camera;
		return setting;
	}
}