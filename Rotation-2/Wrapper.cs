namespace Roation;

public class Wrapper<T>(T pV) where T: struct {
	private T _value = pV;
	public ref T V => ref _value;
}