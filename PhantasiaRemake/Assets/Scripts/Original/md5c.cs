using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class md5c //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //    /* MD5 context. */    
        public class MD5_CTX
        {
            //online spec
            public ulong[] i; //[2]
            public ulong[] buf;//[4];
            public char[] Cin;//[64];
            public char[] digest;//[16];

            //phantasia version
            public uint[] count;//[2];                /* number of bits, modulo 2^64 (lsb first) */
            public uint[] state;//[4];                                           /* state (ABCD) */
            public char[] buffer;//[64]; //unsigned                                 /* input buffer */


            public MD5_CTX()
            {
                i = new ulong[2];
                buf = new ulong[4];
                Cin = new char[64];
                digest = new char[16];

                count = new uint[2];
                state = new uint[4];
                buffer = Cin;
            }
        }


        /* Constants for MD5Transform routine.
         */
        const int S11 = 7;
        const int S12 = 12;
        const int S13 = 17;
        const int S14 = 22;
        const int S21 = 5;
        const int S22 = 9;
        const int S23 = 14;
        const int S24 = 20;
        const int S31 = 4;
        const int S32 = 11;
        const int S33 = 16;
        const int S34 = 23;
        const int S41 = 6;
        const int S42 = 10;
        const int S43 = 15;
        const int S44 = 21;

        //    static void MD5Transform PROTO_LIST((UINT4[4], unsigned char[64]));
        //static void Encode PROTO_LIST((unsigned char*, UINT4*, unsigned int));
        //static void Decode PROTO_LIST((UINT4*, unsigned char*, unsigned int));
        //static void MD5_memcpy PROTO_LIST((POINTER, POINTER, unsigned int));
        //static void MD5_memset PROTO_LIST((POINTER, int, unsigned int));

        static char[] PADDING = new char[] {
  //0x80 //todo: debug
            '1',
            '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0',
  '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0',
  '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0'
};

        /* F, G, H and I are basic MD5 functions. */
        static uint F(uint x, uint y, uint z)
        {
            //uint test1 = ((x) & (y));
            //uint test2 = ((~x) & (z));
            //uint test3 = test1 | test2;
            return ((x) & (y)) | ((~x) & (z));
            //int xi = Convert.ToInt32(x);
            //int yi = Convert.ToInt32(y);
            //int zi = Convert.ToInt32(z);
            //return ((xi) & (yi)) | ((~xi) & (zi));
        }
        static uint G(uint x, uint y, uint z)
        {
            //int xi = Convert.ToInt32(x);
            //int yi = Convert.ToInt32(y);
            //int zi = Convert.ToInt32(z);
            //return ((xi) & (zi)) | ((yi) & (~zi));
            return ((x) & (z)) | ((y) & (~z));
        }
        static uint H(uint x, uint y, uint z)
        {
            //int xi = Convert.ToInt32(x);
            //int yi = Convert.ToInt32(y);
            //int zi = Convert.ToInt32(z);
            //return ((xi) ^ (yi) ^ (zi));
            return ((x) ^ (y) ^ (z));
        }
        static uint I(uint x, uint y, uint z)
        {
            //int xi = Convert.ToInt32(x);
            //int yi = Convert.ToInt32(y);
            //int zi = Convert.ToInt32(z);
            //return ((yi) ^ ((xi) | (~zi)));
            return ((y) ^ ((x) | (~z)));
        }

        /* ROTATE_LEFT rotates x left n bits. */
        static uint ROTATE_LEFT(uint x, uint n)
        {
            //int xi = Convert.ToInt32(x);
            int ni = Convert.ToInt32(n);
            //return (((xi) << (ni)) | ((xi) >> (32 - (ni))));
            return (((x) << (ni)) | ((x) >> (32 - (ni))));
        }

        /* FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4.
           Rotation is separate from addition to prevent recomputation. */
        static uint FF(uint a, uint b, uint c, uint d, uint x, uint s, uint ac) { //\
            (a) += Convert.ToUInt32(F((b), (c), (d)) + (x) + ac);// (uint)(ac); //\ //todo: all the \ ?
            (a) = Convert.ToUInt32(ROTATE_LEFT((a), (s))); //\
            (a) += (b); //\
            return a; // this return is an assumption
          }
        static uint GG(uint a, uint b, uint c, uint d, uint x, uint s, uint ac) { //\
            (a) += Convert.ToUInt32(G((b), (c), (d)) + (x) + ac);// (uint)(ac); //\
            (a) = Convert.ToUInt32(ROTATE_LEFT((a), (s))); //\
            (a) += (b); //\
            return a; // this return is an assumption
        }
        static uint HH(uint a, uint b, uint c, uint d, uint x, uint s, uint ac) { //\
            (a) += Convert.ToUInt32(H((b), (c), (d)) + (x) + ac);// (uint)(ac);// \
            (a) = Convert.ToUInt32(ROTATE_LEFT((a), (s)));// \
            (a) += (b); //\
            return a; // this return is an assumption
        }
        static uint II(uint a, uint b, uint c, uint d, uint x, uint s, uint ac) { //\
            (a) += Convert.ToUInt32(I((b), (c), (d)) + (x) + ac);// (uint)(ac);//\
            (a) = Convert.ToUInt32(ROTATE_LEFT((a), (s))); //\
            (a) += (b); //\
            return a; // this return is an assumption
        }

        /* MD5 initialization. Begins an MD5 operation, writing a new context. */
        public static void MD5Init(ref MD5_CTX context) //debug
        {
            context.count[0] = context.count[1] = 0;

            /* Load magic initialization constants. */
            context.state[0] = 0x67452301;
            context.state[1] = 0xefcdab89;
            context.state[2] = 0x98badcfe;
            context.state[3] = 0x10325476;
        }

        /* MD5 block update operation. Continues an MD5 message-digest operation,
             processing another message block, and updating the context. */
        public static void MD5Update(ref MD5_CTX context,  //debug
            char[] input, uint inputLen)
        {
            uint i, index, partLen;

            /* Compute number of bytes mod 64 */
            index = (uint)((context.count[0] >> 3) & 0x3F);

            /* Update number of bits */
            if ((context.count[0] += ((uint)inputLen << 3)) < ((uint)inputLen << 3))
                context.count[1]++;
            context.count[1] += ((uint)inputLen >> 29);

            partLen = 64 - index;

            /* Transform as many times as possible. */
            if (inputLen >= partLen)
            {
                //MD5_memcpy((POINTER) & context.buffer[index], (POINTER)input, partLen); //todo
                MD5Transform(context.state, context.buffer);

                for (i = partLen; i + 63 < inputLen; i += 64)
                    MD5Transform(context.state, new char[1] { input[Convert.ToInt32(i)] });

                index = 0;
            }
            else
                i = 0;

            /* Buffer remaining input */
            //MD5_memcpy((POINTER) & context.buffer[index], (POINTER) & input[i], inputLen - i); //todo
        }

        /* MD5 finalization. Ends an MD5 message-digest operation, writing the
             the message digest and zeroizing the context.
         */
        public static void MD5Final(ref char[] digest, ref MD5_CTX context)//debug
        {
            char[] bits = new char[8];
            uint index, padLen;

            /* Save number of bits */
            Encode(bits, context.count, 8);

            /* Pad out to 56 mod 64.
             */
            index = (uint)((context.count[0] >> 3) & 0x3f);
            padLen = (index < 56) ? (56 - index) : (120 - index);
            MD5Update(ref context, PADDING, padLen);

            /* Append length (before padding) */
            MD5Update(ref context, bits, 8);

            /* Store state in digest */
            Encode(digest, context.state, 16);

            /* Zeroize sensitive information.
             */
            //MD5_memset((POINTER)context, 0, sizeof(* context)); //todo
        }

        /* MD5 basic transformation. Transforms state based on block.
         */
        static void MD5Transform(uint[] state,//debug //todo: these uint[]s were UINT4 originally. any difference?
            char[] block)
        {
            uint a = state[0], b = state[1], c = state[2], d = state[3];
            uint[] x = new uint[16];


            Decode(x, block, 64);

            /* Round 1 */
            FF(a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
            FF(d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
            FF(c, d, a, b, x[2], S13, 0x242070db); /* 3 */
            FF(b, c, d, a, x[3], S14, 0xc1bdceee); /* 4 */
            FF(a, b, c, d, x[4], S11, 0xf57c0faf); /* 5 */
            FF(d, a, b, c, x[5], S12, 0x4787c62a); /* 6 */
            FF(c, d, a, b, x[6], S13, 0xa8304613); /* 7 */
            FF(b, c, d, a, x[7], S14, 0xfd469501); /* 8 */
            FF(a, b, c, d, x[8], S11, 0x698098d8); /* 9 */
            FF(d, a, b, c, x[9], S12, 0x8b44f7af); /* 10 */
            FF(c, d, a, b, x[10], S13, 0xffff5bb1); /* 11 */
            FF(b, c, d, a, x[11], S14, 0x895cd7be); /* 12 */
            FF(a, b, c, d, x[12], S11, 0x6b901122); /* 13 */
            FF(d, a, b, c, x[13], S12, 0xfd987193); /* 14 */
            FF(c, d, a, b, x[14], S13, 0xa679438e); /* 15 */
            FF(b, c, d, a, x[15], S14, 0x49b40821); /* 16 */

            /* Round 2 */
            GG(a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
            GG(d, a, b, c, x[6], S22, 0xc040b340); /* 18 */
            GG(c, d, a, b, x[11], S23, 0x265e5a51); /* 19 */
            GG(b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
            GG(a, b, c, d, x[5], S21, 0xd62f105d); /* 21 */
            GG(d, a, b, c, x[10], S22, 0x2441453); /* 22 */
            GG(c, d, a, b, x[15], S23, 0xd8a1e681); /* 23 */
            GG(b, c, d, a, x[4], S24, 0xe7d3fbc8); /* 24 */
            GG(a, b, c, d, x[9], S21, 0x21e1cde6); /* 25 */
            GG(d, a, b, c, x[14], S22, 0xc33707d6); /* 26 */
            GG(c, d, a, b, x[3], S23, 0xf4d50d87); /* 27 */
            GG(b, c, d, a, x[8], S24, 0x455a14ed); /* 28 */
            GG(a, b, c, d, x[13], S21, 0xa9e3e905); /* 29 */
            GG(d, a, b, c, x[2], S22, 0xfcefa3f8); /* 30 */
            GG(c, d, a, b, x[7], S23, 0x676f02d9); /* 31 */
            GG(b, c, d, a, x[12], S24, 0x8d2a4c8a); /* 32 */

            /* Round 3 */
            HH(a, b, c, d, x[5], S31, 0xfffa3942); /* 33 */
            HH(d, a, b, c, x[8], S32, 0x8771f681); /* 34 */
            HH(c, d, a, b, x[11], S33, 0x6d9d6122); /* 35 */
            HH(b, c, d, a, x[14], S34, 0xfde5380c); /* 36 */
            HH(a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
            HH(d, a, b, c, x[4], S32, 0x4bdecfa9); /* 38 */
            HH(c, d, a, b, x[7], S33, 0xf6bb4b60); /* 39 */
            HH(b, c, d, a, x[10], S34, 0xbebfbc70); /* 40 */
            HH(a, b, c, d, x[13], S31, 0x289b7ec6); /* 41 */
            HH(d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
            HH(c, d, a, b, x[3], S33, 0xd4ef3085); /* 43 */
            HH(b, c, d, a, x[6], S34, 0x4881d05); /* 44 */
            HH(a, b, c, d, x[9], S31, 0xd9d4d039); /* 45 */
            HH(d, a, b, c, x[12], S32, 0xe6db99e5); /* 46 */
            HH(c, d, a, b, x[15], S33, 0x1fa27cf8); /* 47 */
            HH(b, c, d, a, x[2], S34, 0xc4ac5665); /* 48 */

            /* Round 4 */
            II(a, b, c, d, x[0], S41, 0xf4292244); /* 49 */
            II(d, a, b, c, x[7], S42, 0x432aff97); /* 50 */
            II(c, d, a, b, x[14], S43, 0xab9423a7); /* 51 */
            II(b, c, d, a, x[5], S44, 0xfc93a039); /* 52 */
            II(a, b, c, d, x[12], S41, 0x655b59c3); /* 53 */
            II(d, a, b, c, x[3], S42, 0x8f0ccc92); /* 54 */
            II(c, d, a, b, x[10], S43, 0xffeff47d); /* 55 */
            II(b, c, d, a, x[1], S44, 0x85845dd1); /* 56 */
            II(a, b, c, d, x[8], S41, 0x6fa87e4f); /* 57 */
            II(d, a, b, c, x[15], S42, 0xfe2ce6e0); /* 58 */
            II(c, d, a, b, x[6], S43, 0xa3014314); /* 59 */
            II(b, c, d, a, x[13], S44, 0x4e0811a1); /* 60 */
            II(a, b, c, d, x[4], S41, 0xf7537e82); /* 61 */
            II(d, a, b, c, x[11], S42, 0xbd3af235); /* 62 */
            II(c, d, a, b, x[2], S43, 0x2ad7d2bb); /* 63 */
            II(b, c, d, a, x[9], S44, 0xeb86d391); /* 64 */

            state[0] += a;
            state[1] += b;
            state[2] += c;
            state[3] += d;

            //  /* Zeroize sensitive information.
            //   */
            //  MD5_memset((POINTER) x, 0, sizeof (x)); //todo
        }

        /* Encodes input (UINT4) into output (unsigned char). Assumes len is
             a multiple of 4.
         */
        static void Encode(char[] output, uint[] input, //debug
            uint len)
        {
            string inputstring = "";
            for (int k = 0; k < input.Length; k++)
            {
                inputstring += input[k] + ",";
            }

            Debug.LogError("md5 Encode: output: " + new string(output).Replace('\0', '£')
                + " || input: " + inputstring
                + " || len: " + len + " ||");

            uint i, j;
            for (i = 0, j = 0; j < len; i++, j += 4)
            {
                output[j] = (char)(input[i] & 0xff); //todo: these char were unsigned char originally. any difference?
            output[j + 1] = (char)((input[i] >> 8) & 0xff);
            output[j + 2] = (char)((input[i] >> 16) & 0xff);
            output[j + 3] = (char)((input[i] >> 24) & 0xff);
        }
    }

        /* Decodes input (unsigned char) into output (UINT4). Assumes len is
             a multiple of 4.
         */
        static void Decode(uint[] output, //debug
            char[] input, uint len)
        {
            uint i, j;

            for (i = 0, j = 0; j < len; i++, j += 4)
                output[i] = ((uint)input[j]) | (((uint)input[j + 1]) << 8) |
                  (((uint)input[j + 2]) << 16) | (((uint)input[j + 3]) << 24);
        }

        /* Note: Replace "for loop" with standard memcpy if possible.
         */
        static void MD5_memcpy(object[] output, object[] input, uint len)
        {
            uint i;

            for (i = 0; i < len; i++)
                output[i] = input[i];
        }

        /* Note: Replace "for loop" with standard memset if possible.
         */
        static void MD5_memset(ref char[] output, int value, uint len)
        {
            uint i;

            for (i = 0; i < len; i++)
                (output)[i] = (char)value;
        }
    }
}
