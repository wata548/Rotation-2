namespace Rotation;

public class Program {
    public static Logic Logic;
	
    public static void Main() {
        Console.Write("Frame? (60): ");
        if(!int.TryParse(Console.ReadLine()??"", out var frame)) frame = 60; 
        Console.Write("Ascii? (y / N): ");
        var ascii = (Console.ReadLine()??"").Contains('y');
        Console.Write("Isolate? (y / N): ");
        var isolate = (Console.ReadLine()??"").Contains('y');
        Console.Write("Enter scene name(LoadFbxScene): ");
        var sceneName = Console.ReadLine()??"";
        if (string.IsNullOrWhiteSpace(sceneName))
	        sceneName = "LoadFbxScene";
        var sceneType = Type.GetType($"Rotation.Scene.{sceneName}");
		
        var setting = new Setting(
            new(13,13),
            Vector.Zero, 
            //new(250, 213, 27), yellow
            new Color(),
            frame,
            20, 
            Fog: 0.1f,
            FOV: 109,
            Isolate: isolate,
            UseColor: !ascii,
            FillContext: true,
            ZBufferShading: false
        );
		
        var scene =  Activator.CreateInstance(sceneType, []) as IScene;
        if (scene == null) throw new ArgumentException($"{sceneName} isn't exist. Check again");
        Logic = new Logic(setting, scene);	
		
        Logic.StartDataLoop();
        Logic.StartRenderLoop().Wait();
		
    }
}