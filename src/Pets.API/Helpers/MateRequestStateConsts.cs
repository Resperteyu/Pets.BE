namespace Pets.API.Helpers
{
    public static class MateRequestStateConsts
    {
        public static byte SENT = 1;
        public static byte CHANGES_REQUESTED = 2;
        public static byte ACCEPTED = 3;
        public static byte BREEDING = 4;
        public static byte REJECTED = 5;
        public static byte COMPLETED = 6;
        public static byte FAILED = 7;
    }
}