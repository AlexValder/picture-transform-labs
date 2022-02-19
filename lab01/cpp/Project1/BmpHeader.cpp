#include "BmpHeader.h"
#include <string>
#include <fstream>

#pragma region CTORS & =

BmpHeader::BmpHeader(const char* file) {
    this->_name = std::string(file);
    std::ifstream stream(file, std::ios::binary);

    if (stream.is_open()) {
        parse_file(stream);
    }
    else {
        throw std::runtime_error(">:(");
    }
    stream.close();
}

BmpHeader::BmpHeader(const std::string& file) : BmpHeader(file.c_str()) {}

BmpHeader::BmpHeader(const BmpHeader& other) {
    copy_others(other);
}

BmpHeader::BmpHeader(BmpHeader&& other) noexcept {
    move_others(std::move(other));
}

BmpHeader& BmpHeader::operator=(const BmpHeader& other) {
    copy_others(other);

    return *this;
}

BmpHeader& BmpHeader::operator=(BmpHeader&& other) noexcept {
    move_others(std::move(other));

    return *this;
}

#pragma endregion

#pragma region CTOR HELPERS

void BmpHeader::parse_file(std::ifstream& stream) {
    if (!stream.is_open() || stream.bad()) {
        throw std::runtime_error("Failed to create a BmpImage");
    }

    parse_header(stream);
    parse_infoheader(stream);
    parse_colortable(stream);
}

template <class T>
void read(std::ifstream& stream, T& variable) {
    char buffer[sizeof(T)];
    stream.read(buffer, sizeof(T));
    variable = *reinterpret_cast<T*>(buffer);
}

void read_int(std::ifstream& stream, int& variable) {
    read<int>(stream, variable);
}

void read_short(std::ifstream& stream, short& variable) {
    read<short>(stream, variable);
}

void BmpHeader::parse_header(std::ifstream& stream) {
    char buffer[4];

    stream.read(buffer, 2);
    buffer[2] = 0;
    buffer[3] = 0;
    if (strcmp(buffer, "BM") != 0) {
        throw std::runtime_error("Not a BMP image");
    }

    read_int(stream, this->_fileSize);
    stream.ignore(4);
    read_int(stream, this->_dataOffset);
}

void BmpHeader::parse_infoheader(std::ifstream& stream) {
    stream.ignore(4);
    read_int(stream, this->_width);
    read_int(stream, this->_height);
    read_short(stream, this->_planes);
    read_short(stream, this->_bitCount);
    this->_numColors = std::pow<long long>(2, this->_bitCount);
    read_int(stream, this->_compression);
    read_int(stream, this->_imageSize);
    read_int(stream, this->_hResolution);
    read_int(stream, this->_vResolution);
    read_int(stream, this->_colorsUsed);
    read_int(stream, this->_colorsImportant);
}

void BmpHeader::parse_colortable(std::ifstream& stream) {
    if (this->_bitCount > 8) {
        return;
    }

    char buffer[sizeof(4)];
    for (int i = 0; i < this->_numColors; ++i) {
        stream.read(buffer, 4);
        this->_colorTable.push_back(*reinterpret_cast<BmpColor*>(buffer));
    }
}

void BmpHeader::copy_others(const BmpHeader& other) {
    this->_name = other._name;
    this->_bitCount = other._bitCount;
    this->_colorsImportant = other._colorsImportant;
    this->_colorsUsed = other._colorsUsed;
    this->_colorTable = other._colorTable;
    this->_compression = other._compression;
    this->_dataOffset = other._dataOffset;
    this->_fileSize = other._fileSize;
    this->_height = other._height;
    this->_hResolution = other._hResolution;
    this->_imageSize = other._imageSize;
    this->_numColors = other._numColors;
    this->_planes = other._planes;
    this->_vResolution = other._vResolution;
    this->_width = other._width;
}

void BmpHeader::move_others(BmpHeader&& other) noexcept {
    this->_name = std::move(other._name);
    this->_bitCount = std::move(other._bitCount);
    this->_colorsImportant = std::move(other._colorsImportant);
    this->_colorsUsed = std::move(other._colorsUsed);
    this->_colorTable = std::move(other._colorTable);
    this->_compression = std::move(other._compression);
    this->_dataOffset = std::move(other._dataOffset);
    this->_fileSize = std::move(other._fileSize);
    this->_height = std::move(other._height);
    this->_hResolution = std::move(other._hResolution);
    this->_imageSize = std::move(other._imageSize);
    this->_numColors = std::move(other._numColors);
    this->_planes = std::move(other._planes);
    this->_vResolution = std::move(other._vResolution);
    this->_width = std::move(other._width);
}

#pragma endregion

#pragma region GETTERS

int BmpHeader::get_file_size() const noexcept {
    return this->_fileSize;
}

int BmpHeader::get_width() const noexcept {
    return this->_width;
}

int BmpHeader::get_height() const noexcept {
    return this->_height;
}

short BmpHeader::get_bit_count() const noexcept {
    return this->_bitCount;
}

long long BmpHeader::get_color_nums() const noexcept {
    return this->_numColors;
}

int BmpHeader::get_compression() const noexcept {
    return this->_compression;
}

int BmpHeader::get_horizontal_resolution() const noexcept {
    return this->_hResolution;
}

int BmpHeader::get_vertical_resolution() const noexcept {
    return this->_vResolution;
}

int BmpHeader::get_colors_used() const noexcept {
    return this->_colorsUsed;
}

int BmpHeader::get_colors_important() const noexcept {
    return this->_colorsImportant;
}

const char* BmpHeader::get_name() const noexcept {
    return this->_name.c_str();
}

#pragma endregion
