#include "Input.h"

#include <windows.h>
#include <iostream>

POINT p;
bool KeyDown(int c) {
    return GetAsyncKeyState(c) & 0x8000;
}
float GetMouseX() {
    GetCursorPos(&p);
    auto width =  GetSystemMetrics(SM_CXSCREEN);
    return (float)p.x / width;
}
float GetMouseY() {
    GetCursorPos(&p);
    auto height =  GetSystemMetrics(SM_CYSCREEN);
    return 1.f - (float)p.y / height;
}
