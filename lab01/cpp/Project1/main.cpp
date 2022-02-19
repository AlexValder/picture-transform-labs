#include <iostream>
#include <fstream>
#include <string>
#include <Windows.h>
#include <gdiplus.h>
#include <conio.h>
#pragma comment(lib, "Gdiplus")
#include "BmpHeader.h"

void print_header_info(const BmpHeader& header);

int main() {
    const std::pair<std::string, std::string> filepath[] = {
        std::make_pair("poland.bmp", "00_poland.bmp"),
        std::make_pair("cat.bmp", "00_cat.bmp"),
        std::make_pair("game.bmp", "00_game.bmp")
    };

    Gdiplus::GdiplusStartupInput gdiplusStartupInput;
    ULONG_PTR gdiplusToken;
    GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

    CLSID cslid;
    HRESULT res = CLSIDFromString(L"{557cf400-1a04-11d3-9a73-0000f81ef32e}", &cslid);
    if (!SUCCEEDED(res)) {
        std::cerr << ">:( GDI+ hates CLSID" << std::endl;
        return -1;
    }

    for (const auto& file : filepath) {
        const auto input = std::wstring(file.first.begin(), file.first.end());
        const auto output = std::wstring(file.second.begin(), file.second.end());

        print_header_info(BmpHeader(file.first));
        Gdiplus::Bitmap bitmap(input.c_str());

        const auto width = bitmap.GetWidth();
        const auto height = min(bitmap.GetHeight(), 30);
        for (int i = 0; i < width; ++i) {
            for (int j = 0; j < height; ++j) {
                bitmap.SetPixel(i, j, Gdiplus::Color::Red);
            }
        }

        std::cout << "Saving to " << file.second << "..." << std::endl << std::endl;
        bitmap.Save(output.c_str(), &cslid);
    }

    Gdiplus::GdiplusShutdown(gdiplusToken);

    const auto _ = _getch();
    return 0;
}

void print_header_info(const BmpHeader& header) {
    std::cout
        << "Name: " << header.get_name() << std::endl
        << "Width (pixels): " << header.get_width() << std::endl
        << "Height (pixels): " << header.get_height() << std::endl
        << "Color depth: " << header.get_bit_count() << std::endl
        << "Number of colors: " << header.get_color_nums() << std::endl;
}
