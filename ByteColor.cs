using Raylib_cs;

namespace Paint;

struct ByteColor {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public ByteColor() : this(0, 0, 0, 0) { }                               // No parameters = tranasparent
    public ByteColor(byte r, byte g, byte b) : this(r, g, b, 255) { }       // No alpha, assume 100% opaque
    public ByteColor(byte r, byte g, byte b, byte a) {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public void Lighten(int amount) {
        R = (byte)Math.Clamp(R + amount, 0, 255);
        G = (byte)Math.Clamp(G + amount, 0, 255);
        B = (byte)Math.Clamp(B + amount, 0, 255);
    }

    public void Darken(int amount) {
        Lighten(-amount);
    }

    public Color ToRL() {
        return new Color(R, G, B, A);
    }
}