namespace Rotation.Light;

public class CelSpotLight(float pMin = 0.2f, float pDiv = 0.49f):SpotLight {
	private readonly float _div = pDiv;
	private readonly float _min = pMin;
	
	protected override float PostProcessing(float pV) =>
		MathF.Floor(pV / _div) * _div * (1 - _min) + _min;
}