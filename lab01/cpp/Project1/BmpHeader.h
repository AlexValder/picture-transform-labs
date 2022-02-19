#pragma once
#include <vector>
#include <string>

class BmpHeader final {
public:
    typedef struct BmpColor {
        char red, green, blue, _reserved;
    };

    BmpHeader(const char*);
    BmpHeader(const std::string&);
    BmpHeader(const BmpHeader&);
    BmpHeader(BmpHeader&&) noexcept;

    int get_file_size() const noexcept;
    int get_width() const noexcept;
    int get_height() const noexcept;
    short get_bit_count() const noexcept;
    long long get_color_nums() const noexcept;
    int get_compression() const noexcept;
    int get_horizontal_resolution() const noexcept;
    int get_vertical_resolution() const noexcept;
    int get_colors_used() const noexcept;
    int get_colors_important() const noexcept;
    const char* get_name() const noexcept;

    BmpHeader& operator=(const BmpHeader&);
    BmpHeader& operator=(BmpHeader&&) noexcept;

    ~BmpHeader() noexcept = default;
private:
    int _fileSize, _dataOffset, _width, _height,
        _compression, _imageSize, _hResolution,
        _vResolution, _colorsUsed, _colorsImportant;
    short _planes, _bitCount;
    long long _numColors;
    std::string _name;
    std::vector<BmpColor> _colorTable;

    void parse_header(std::ifstream&);
    void parse_infoheader(std::ifstream&);
    void parse_colortable(std::ifstream&);
    void parse_file(std::ifstream&);
    void copy_others(const BmpHeader&);
    void move_others(BmpHeader&&) noexcept;
};

