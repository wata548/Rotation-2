#include "Input.h"

#include <mutex>

POINT mousePoint;
std::mutex delta_mutex;
POINT mouse_delta;

bool KeyDown(int c)
{
    return GetAsyncKeyState(c) & 0x8000;
}

float GetMouseX()
{
    GetCursorPos(&mousePoint);
    auto width = GetSystemMetrics(SM_CXSCREEN);
    return (float)mousePoint.x / width;
}

float GetMouseY()
{
    GetCursorPos(&mousePoint);
    auto height = GetSystemMetrics(SM_CYSCREEN);
    return 1.f - (float)mousePoint.y / height;
}

/*LRESULT CALLBACK A(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
    switch (msg)
    {
    case WM_INPUT:
        RAWINPUT raw{};
        UINT size = sizeof(raw);

        auto inputData = GetRawInputData(
            reinterpret_cast<HRAWINPUT>(lparam),
            RID_INPUT, &raw, &size,
            sizeof(HRAWINPUT)
        );

        if (inputData != -1u) {
            if (raw.header.dwType == RIM_TYPEMOUSE) {
                std::lock_guard<std::mutex> lock(delta_mutex);
                mouse_delta.x += raw.data.mouse.lLastX;
                mouse_delta.y += raw.data.mouse.lLastY;
            }
        }
        break;
    case WM_CLOSE:
        DestroyWindow(hwnd);
        return 0;
    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    }
    return DefWindowProc(hwnd, msg, wparam, lparam);
}*/
