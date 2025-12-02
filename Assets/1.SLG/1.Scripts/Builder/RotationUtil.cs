public static class RotationUtil
{
    // rotationDegrees = 0, 90, 180, 270
    public static void RotateCell(int i, int j, int size, int rotationDegrees, out int ri, out int rj)
    {
        int rot = ((rotationDegrees % 360) + 360) % 360;
        int k = rot / 90; // 0~3

        switch (k)
        {
            case 0: // 0°
                ri = i;
                rj = j;
                break;

            case 1: // 90°
                ri = j;
                rj = size - 1 - i;
                break;

            case 2: // 180°
                ri = size - 1 - i;
                rj = size - 1 - j;
                break;

            case 3: // 270°
                ri = size - 1 - j;
                rj = i;
                break;

            default:
                ri = i; rj = j; break;
        }
    }
}
