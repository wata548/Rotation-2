using Rotation.Input;

namespace Rotation;

public class Program {
	public static Logic Logic;
	public static void Main() {
		var setting = new Setting(
			ScreenRange: new(13, 13),
			DefaultColor: new("555555"),
			Frame: 60,
			CoordDetail: 10,
			CastShadow: true,
			ApplyNormalMap: true,
			Ascii: false,
			Isolate: false,
			Fog: 0.05f,
			ZBufferShading: false,
			LightProcessType: LightProcessType.Screen
		) {
			Camera = new Camera{
				FOV = 109, 
				Pos = Vector.Backward * 4,
			}
		};

		if (!BoolInput("Use default setting", true)) {
			setting = setting with {
				Frame = ValueInput("Frame", setting.Frame),
				CoordDetail = ValueInput("Detail", setting.CoordDetail),
				CastShadow = BoolInput("CastShadow", setting.CastShadow),
				ApplyNormalMap = BoolInput("Use normal map", setting.ApplyNormalMap),
				Ascii = BoolInput("Ascii", setting.Ascii),
				Isolate = BoolInput("Isolate", setting.Isolate),
			};	
		}

		var a = setting with { };
		var sceneName = ValueInput("Scene name", "LoadFbxScene");
		var sceneType = Type.GetType($"Rotation.Scene.{sceneName}");
		
		if (sceneType == null) throw new ArgumentException($"{sceneName} isn't exist. Check again");
		var scene =  Activator.CreateInstance(sceneType, setting) as IScene;
		if (sceneType == null) throw new ArgumentException($"{sceneName} isn't IScene type. Check again");
		Logic = new Logic(setting, scene!);	
		
		Logic.StartDataLoop();
		Logic.StartRenderLoop().Wait();
		
	}
	
	//==================================================Methods	
	private static T ValueInput<T>(string pContext, T pDefault) where T: IParsable<T> {
		Console.Write($"{pContext}? ({pDefault}): ");
		var v = Console.ReadLine() ?? "";
		if (string.IsNullOrWhiteSpace(v)) return pDefault;
		try {
			return T.Parse(v, null);
		}
		catch {
			return pDefault;
		}
	}
	private static bool BoolInput(string pContext, bool pDefault) {
		Console.Write($"{pContext}? ({(pDefault ? "Y / n" : "y / N")}): ");
		if(pDefault)
			return !(Console.ReadLine()??"").Contains('n');
		return (Console.ReadLine()??"").Contains('y');
	}   
}