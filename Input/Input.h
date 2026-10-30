#ifndef INPUT_LIBRARY_H
#define INPUT_LIBRARY_H

extern "C" {
__declspec(dllexport) bool KeyDown(int);
__declspec(dllexport) float GetMouseX();
__declspec(dllexport) float GetMouseY();
}
#endif // INPUT_LIBRARY_H
