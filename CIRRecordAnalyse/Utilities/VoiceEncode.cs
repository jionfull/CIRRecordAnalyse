using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CIRRecordAnalyse.Utilities
{
    class VoiceEncode
    {
        // Methods
        public byte[] Alaw_Compress(short[] inbuf, long lenth)
        {
            byte[] outbuf = new byte[lenth];
            for (long n = 0; n < lenth; n++)
            {
                short ix = (inbuf[(int)((IntPtr)n)] < 0) ? ((short)(~inbuf[(int)((IntPtr)n)] >> 4)) : ((short)(inbuf[(int)((IntPtr)n)] >> 4));
                if (ix > 15)
                {
                    short iexp = 1;
                    while (ix > 0x1f)
                    {
                        ix = (short)(ix >> 1);
                        iexp = (short)(iexp + 1);
                    }
                    ix = (short)(ix - 0x10);
                    ix = (short)(ix + ((short)(iexp << 4)));
                }
                if (inbuf[(int)((IntPtr)n)] >= 0)
                {
                    ix = (short)(ix | 0x80);
                }
                outbuf[(int)((IntPtr)n)] = (byte)(ix ^ 0x55);
            }
            return outbuf;
        }

        public short[] Alaw_Expand(byte[] inbuf, long lenth)
        {
            short[] outbuf = new short[lenth];
            for (long n = 0; n < lenth; n++)
            {
                short ix = (short)(inbuf[(int)((IntPtr)n)] ^ 0x55);
                ix = (short)(ix & 0x7f);
                short iexp = (short)(ix >> 4);
                short mant = (short)(ix & 15);
                if (iexp > 0)
                {
                    mant = (short)(mant + 0x10);
                }
                mant = (short)((mant << 4) + 8);
                if (iexp > 1)
                {
                    mant = (short)(mant << (iexp - 1));
                }
                outbuf[(int)((IntPtr)n)] = (short)((inbuf[(int)((IntPtr)n)] > 0x7f) ? mant : -mant);
            }
            return outbuf;
        }

        public byte[] G711ToG726Encode(byte[] linbuffer, int length)
        {
            int i;
            G726_STATE states = new G726_STATE();
            short[] shortarray1 = new short[length];
            for (i = 0; i < length; i++)
            {
                shortarray1[i] = linbuffer[i];
            }
            short[] shortarray2 = this.G726_encode(shortarray1, (long)length, '1', 4, 1, states);
            byte[] outbuffer = new byte[length / 2];
            for (i = 0; i < (length / 2); i++)
            {
                outbuffer[i] = (byte)((shortarray2[2 * i] & 15) + ((shortarray2[(2 * i) + 1] & 15) << 4));
            }
            return outbuffer;
        }

        public byte[] G711ToPCMDecode(byte[] linbuffer, int length)
        {
            short[] smpbyte = this.Alaw_Expand(linbuffer, (long)length);
            byte[] outbuffer = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                byte[] tmp1 = BitConverter.GetBytes(smpbyte[i]);
                outbuffer[i * 2] = tmp1[0];
                outbuffer[(i * 2) + 1] = tmp1[1];
            }
            return outbuffer;
        }

        private void G726_accum(short wa1, short wa2, short wb1, short wb2, short wb3, short wb4, short wb5, short wb6, ref short se, ref short sez)
        {
            ulong wa11 = (ulong)wa1;
            ulong wa21 = (ulong)wa2;
            ulong wb11 = (ulong)wb1;
            ulong wb21 = (ulong)wb2;
            ulong wb31 = (ulong)wb3;
            ulong wb41 = (ulong)wb4;
            ulong wb51 = (ulong)wb5;
            ulong wb61 = (ulong)wb6;
            ulong sezi = (((((((((wb11 + wb21) & 0xffff) + wb31) & 0xffff) + wb41) & 0xffff) + wb51) & 0xffff) + wb61) & 0xffff;
            ulong sei = (((sezi + wa21) & 0xffff) + wa11) & 0xffff;
            sez = (short)(sezi >> 1);
            se = (short)(sei >> 1);
        }

        private short G726_adda(short dqln, short y)
        {
            return (short)((dqln + (y >> 2)) & 0xfff);
        }

        private short G726_addb(short dq, short se)
        {
            ulong dq1 = (ulong)(dq & 0xffff);
            ulong se1 = (ulong)se;
            ulong dqi = (((short)((dq >> 15) & 1)) == 0) ? dq1 : ((0x10000 - (dq1 & 0x7fff)) & 0xffff);
            ulong sei = (((short)(se >> 14)) == 0) ? se1 : (0x8000 + se1);
            return (short)((dqi + sei) & 0xffff);
        }

        private void G726_addc(short dq, short sez, ref short pk0, ref short sigpk)
        {
            ulong dq1 = (ulong)(dq & 0xffff);
            ulong sez1 = (ulong)sez;
            ulong dqi = (((short)((dq >> 15) & 1)) == 0) ? dq1 : ((0x10000 - (dq1 & 0x7fff)) & 0xffff);
            ulong sezi = (((short)(sez >> 14)) == 0) ? sez1 : (sez1 + 0x8000);
            ulong dqsez = (dqi + sezi) & 0xffff;
            pk0 = (short)(dqsez >> 15);
            sigpk = (dqsez == 0) ? ((short)1) : ((short)0);
        }

        private short G726_antilog(short dql, short dqs)
        {
            long ds = dql >> 11;
            long dex = (dql >> 7) & 15;
            long dmn = dql & 0x7f;
            long dqt = dmn + 0x80;
            long dqmag = (ds > 0) ? 0 : ((dqt << 7) >> ((byte)(14 - dex)));
            return (short)((dqs << 15) + dqmag);
        }

        private short G726_compress(short sr, char law)
        {
            short imag;
            short iesp;
            short ofst1 = 0;
            short sp = 0;
            short iss = (short)(sr >> 15);
            long srr = sr & 0xffff;
            long im = (iss == 0) ? srr : ((0x10000 - srr) & 0x7fff);
            if (law != '1')
            {
                imag = (short)im;
                if (imag > 0x1fde)
                {
                    imag = 0x1fde;
                }
                imag = (short)(imag + 1);
                iesp = 0;
                short ofst = 0x1f;
                if (imag <= ofst)
                {
                    goto Label_0174;
                }
                for (iesp = 1; iesp <= 8; iesp = (short)(iesp + 1))
                {
                    ofst1 = ofst;
                    ofst = (short)(ofst + ((short)(((int)1) << (iesp + 5))));
                    if (imag <= ofst)
                    {
                        break;
                    }
                }
            }
            else
            {
                im = (sr == -32768) ? 2 : im;
                imag = (iss == 0) ? ((short)(im >> 1)) : ((short)((im + 1) >> 1));
                if (iss > 0)
                {
                    imag = (short)(imag - 1);
                }
                if (imag > 0xfff)
                {
                    imag = 0xfff;
                }
                iesp = 7;
                for (long i = 1; i <= 7; i++)
                {
                    imag = (short)(imag + imag);
                    if (imag >= 0x1000)
                    {
                        break;
                    }
                    iesp = (short)(7 - i);
                }
                imag = (short)(imag & 0xfff);
                imag = (short)(imag >> 8);
                sp = (iss == 0) ? ((short)(imag + (iesp << 4))) : ((short)((imag + (iesp << 4)) + 0x80));
                return (short)(sp ^ 0x80);
            }
            imag = (short)(imag - ((short)(ofst1 + 1)));
        Label_0174:
            imag = (short)(imag / ((short)(((int)1) << (iesp + 1))));
            sp = (iss == 0) ? ((short)(imag + (iesp << 4))) : ((short)((imag + (iesp << 4)) + 0x80));
            sp = (short)(sp ^ 0x80);
            return (short)(sp ^ 0x7f);
        }

        public short[] G726_decode(short[] inp_buf, long smpno, char law, short rate, short r, G726_STATE state)
        {
            short[] out_buf = new short[smpno];
            long j = 0;
            while (j < smpno)
            {
                short sr2 = this.G726_delayd(r, state.sr1);
                state.sr1 = this.G726_delayd(r, state.sr0);
                short a2 = this.G726_delaya(r, state.a2r);
                short a1 = this.G726_delaya(r, state.a1r);
                short wa2 = this.G726_fmult(a2, sr2);
                short wa1 = this.G726_fmult(a1, state.sr1);
                short dq6 = this.G726_delayd(r, state.dq5);
                state.dq5 = this.G726_delayd(r, state.dq4);
                state.dq4 = this.G726_delayd(r, state.dq3);
                state.dq3 = this.G726_delayd(r, state.dq2);
                state.dq2 = this.G726_delayd(r, state.dq1);
                state.dq1 = this.G726_delayd(r, state.dq0);
                short b1 = this.G726_delaya(r, state.b1r);
                short b2 = this.G726_delaya(r, state.b2r);
                short b3 = this.G726_delaya(r, state.b3r);
                short b4 = this.G726_delaya(r, state.b4r);
                short b5 = this.G726_delaya(r, state.b5r);
                short b6 = this.G726_delaya(r, state.b6r);
                short wb1 = this.G726_fmult(b1, state.dq1);
                short wb2 = this.G726_fmult(b2, state.dq2);
                short wb3 = this.G726_fmult(b3, state.dq3);
                short wb4 = this.G726_fmult(b4, state.dq4);
                short wb5 = this.G726_fmult(b5, state.dq5);
                short wb6 = this.G726_fmult(b6, dq6);
                short se = 0;
                short sez = 0;
                this.G726_accum(wa1, wa2, wb1, wb2, wb3, wb4, wb5, wb6, ref se, ref sez);
                short dms = this.G726_delaya(r, state.dmsp);
                short dml = this.G726_delaya(r, state.dmlp);
                short ap = this.G726_delaya(r, state.apr);
                short al = this.G726_lima(ap);
                short yu = this.G726_delayb(r, state.yup);
                long yl = this.G726_delayc(r, state.ylp);
                short y = this.G726_mix(al, yu, yl);
                short i = inp_buf[(int)((IntPtr)j)];
                short dqln = 0;
                short dqs = 0;
                this.G726_reconst(rate, ref i, ref dqln, ref dqs);
                short dql = this.G726_adda(dqln, y);
                short dq = this.G726_antilog(dql, dqs);
                short td = this.G726_delaya(r, state.tdr);
                short tr = this.G726_trans(td, yl, dq);
                short fi = this.G726_functf(rate, i);
                state.dmsp = this.G726_filta(fi, dms);
                state.dmlp = this.G726_filtb(fi, dml);
                short wi = this.G726_functw(rate, i);
                short yut = this.G726_filtd(wi, y);
                state.yup = this.G726_limb(yut);
                state.ylp = this.G726_filte(state.yup, yl);
                short pk2 = this.G726_delaya(r, state.pk1);
                state.pk1 = this.G726_delaya(r, state.pk0);
                short sigpk = 0;
                this.G726_addc(dq, sez, ref state.pk0, ref sigpk);
                short sr = this.G726_addb(dq, se);
                state.sr0 = this.G726_floatb(sr);
                state.dq0 = this.G726_floata(dq);
                short sp = this.G726_compress(sr, law);
                short slx = this.G726_expand(sp, law);
                short dx = this.G726_subta(slx, se);
                short dlx = 0;
                short dsx = 0;
                this.G726_log(ref dx, ref dlx, ref dsx);
                short dlnx = this.G726_subtb(dlx, y);
                short sd = this.G726_sync(rate, i, sp, dlnx, dsx, law);
                out_buf[(int)((IntPtr)j)] = sd;
                short a2t = this.G726_upa2(state.pk0, state.pk1, pk2, a2, a1, sigpk);
                short a2p = this.G726_limc(a2t);
                state.a2r = this.G726_trigb(tr, a2p);
                short a1t = this.G726_upa1(state.pk0, state.pk1, a1, sigpk);
                short a1p = this.G726_limd(a1t, a2p);
                state.a1r = this.G726_trigb(tr, a1p);
                short tdp = this.G726_tone(a2p);
                state.tdr = this.G726_trigb(tr, tdp);
                short ax = this.G726_subtc(state.dmsp, state.dmlp, tdp, y);
                short app = this.G726_filtc(ax, ap);
                state.apr = this.G726_triga(tr, app);
                short u1 = this.G726_xor(state.dq1, dq);
                short b1p = this.G726_upb(rate, u1, b1, dq);
                state.b1r = this.G726_trigb(tr, b1p);
                short u2 = this.G726_xor(state.dq2, dq);
                short b2p = this.G726_upb(rate, u2, b2, dq);
                state.b2r = this.G726_trigb(tr, b2p);
                short u3 = this.G726_xor(state.dq3, dq);
                short b3p = this.G726_upb(rate, u3, b3, dq);
                state.b3r = this.G726_trigb(tr, b3p);
                short u4 = this.G726_xor(state.dq4, dq);
                short b4p = this.G726_upb(rate, u4, b4, dq);
                state.b4r = this.G726_trigb(tr, b4p);
                short u5 = this.G726_xor(state.dq5, dq);
                short b5p = this.G726_upb(rate, u5, b5, dq);
                state.b5r = this.G726_trigb(tr, b5p);
                short u6 = this.G726_xor(dq6, dq);
                short b6p = this.G726_upb(rate, u6, b6, dq);
                state.b6r = this.G726_trigb(tr, b6p);
                j++;
                r = 0;
            }
            if (law == '1')
            {
                for (j = 0; j < smpno; j++)
                {
                    out_buf[(int)((IntPtr)j)] = (short)(out_buf[(int)((IntPtr)j)] ^ 0x55);
                }
            }
            return out_buf;
        }

        private short G726_delaya(short r, short x)
        {
            return ((r == 0) ? x : ((short)0));
        }

        private short G726_delayb(short r, short x)
        {
            return ((r == 0) ? x : ((short)0x220));
        }

        private long G726_delayc(short r, long x)
        {
            return ((r == 0) ? x : 0x8800);
        }

        private short G726_delayd(short r, short x)
        {
            return ((r == 0) ? x : ((short)0x20));
        }

        public short[] G726_encode(short[] inp_buf, long smpno, char law, short rate, short r, G726_STATE state)
        {
            long j;
            short[] out_buf = new short[smpno];
            if (law == '1')
            {
                for (j = 0; j < smpno; j++)
                {
                    inp_buf[(int)((IntPtr)j)] = (short)(inp_buf[(int)((IntPtr)j)] ^ 0x55);
                }
            }
            j = 0;
            while (j < smpno)
            {
                short s = inp_buf[(int)((IntPtr)j)];
                short sr2 = this.G726_delayd(r, state.sr1);
                state.sr1 = this.G726_delayd(r, state.sr0);
                short a2 = this.G726_delaya(r, state.a2r);
                short a1 = this.G726_delaya(r, state.a1r);
                short wa2 = this.G726_fmult(a2, sr2);
                short wa1 = this.G726_fmult(a1, state.sr1);
                short dq6 = this.G726_delayd(r, state.dq5);
                state.dq5 = this.G726_delayd(r, state.dq4);
                state.dq4 = this.G726_delayd(r, state.dq3);
                state.dq3 = this.G726_delayd(r, state.dq2);
                state.dq2 = this.G726_delayd(r, state.dq1);
                state.dq1 = this.G726_delayd(r, state.dq0);
                short b1 = this.G726_delaya(r, state.b1r);
                short b2 = this.G726_delaya(r, state.b2r);
                short b3 = this.G726_delaya(r, state.b3r);
                short b4 = this.G726_delaya(r, state.b4r);
                short b5 = this.G726_delaya(r, state.b5r);
                short b6 = this.G726_delaya(r, state.b6r);
                short wb1 = this.G726_fmult(b1, state.dq1);
                short wb2 = this.G726_fmult(b2, state.dq2);
                short wb3 = this.G726_fmult(b3, state.dq3);
                short wb4 = this.G726_fmult(b4, state.dq4);
                short wb5 = this.G726_fmult(b5, state.dq5);
                short wb6 = this.G726_fmult(b6, dq6);
                short se = 0;
                short sez = 0;
                this.G726_accum(wa1, wa2, wb1, wb2, wb3, wb4, wb5, wb6, ref se, ref sez);
                short sl = this.G726_expand(s, law);
                short d = this.G726_subta(sl, se);
                short dms = this.G726_delaya(r, state.dmsp);
                short dml = this.G726_delaya(r, state.dmlp);
                short ap = this.G726_delaya(r, state.apr);
                short al = this.G726_lima(ap);
                short yu = this.G726_delayb(r, state.yup);
                long yl = this.G726_delayc(r, state.ylp);
                short y = this.G726_mix(al, yu, yl);
                short dl = 0;
                short ds = 0;
                this.G726_log(ref d, ref dl, ref ds);
                short dln = this.G726_subtb(dl, y);
                short i = this.G726_quan(rate, dln, ds);
                out_buf[(int)((IntPtr)j)] = i;
                short dqln = 0;
                short dqs = 0;
                this.G726_reconst(rate, ref i, ref dqln, ref dqs);
                short dql = this.G726_adda(dqln, y);
                short dq = this.G726_antilog(dql, dqs);
                short fi = this.G726_functf(rate, i);
                state.dmsp = this.G726_filta(fi, dms);
                state.dmlp = this.G726_filtb(fi, dml);
                short wi = this.G726_functw(rate, i);
                short yut = this.G726_filtd(wi, y);
                state.yup = this.G726_limb(yut);
                state.ylp = this.G726_filte(state.yup, yl);
                short td = this.G726_delaya(r, state.tdr);
                short tr = this.G726_trans(td, yl, dq);
                short pk2 = this.G726_delaya(r, state.pk1);
                state.pk1 = this.G726_delaya(r, state.pk0);
                short sigpk = 0;
                this.G726_addc(dq, sez, ref state.pk0, ref sigpk);
                short sr = this.G726_addb(dq, se);
                state.sr0 = this.G726_floatb(sr);
                state.dq0 = this.G726_floata(dq);
                short a2t = 0;
                a2t = this.G726_upa2(state.pk0, state.pk1, pk2, a2, a1, sigpk);
                short a2p = this.G726_limc(a2t);
                state.a2r = this.G726_trigb(tr, a2p);
                short a1t = this.G726_upa1(state.pk0, state.pk1, a1, sigpk);
                short a1p = this.G726_limd(a1t, a2p);
                state.a1r = this.G726_trigb(tr, a1p);
                short tdp = this.G726_tone(a2p);
                state.tdr = this.G726_trigb(tr, tdp);
                short ax = this.G726_subtc(state.dmsp, state.dmlp, tdp, y);
                short app = this.G726_filtc(ax, ap);
                state.apr = this.G726_triga(tr, app);
                short u1 = this.G726_xor(state.dq1, dq);
                short b1p = this.G726_upb(rate, u1, b1, dq);
                state.b1r = this.G726_trigb(tr, b1p);
                short u2 = this.G726_xor(state.dq2, dq);
                short b2p = this.G726_upb(rate, u2, b2, dq);
                state.b2r = this.G726_trigb(tr, b2p);
                short u3 = this.G726_xor(state.dq3, dq);
                short b3p = this.G726_upb(rate, u3, b3, dq);
                state.b3r = this.G726_trigb(tr, b3p);
                short u4 = this.G726_xor(state.dq4, dq);
                short b4p = this.G726_upb(rate, u4, b4, dq);
                state.b4r = this.G726_trigb(tr, b4p);
                short u5 = this.G726_xor(state.dq5, dq);
                short b5p = this.G726_upb(rate, u5, b5, dq);
                state.b5r = this.G726_trigb(tr, b5p);
                short u6 = this.G726_xor(dq6, dq);
                short b6p = this.G726_upb(rate, u6, b6, dq);
                state.b6r = this.G726_trigb(tr, b6p);
                j++;
                r = 0;
            }
            return out_buf;
        }

        private short G726_expand(short s, char law)
        {
            long mant;
            long iexp;
            short ss;
            short sig;
            short ssq;
            short sss;
            short s1 = s;
            if (law == '1')
            {
                s1 = (short)(s1 ^ 0x80);
                if (s1 >= 0x80)
                {
                    s1 = (short)(s1 + -128);
                    sig = 0x1000;
                }
                else
                {
                    sig = 0;
                }
                iexp = s1 / 0x10;
                mant = s1 - (iexp << 4);
                ss = (iexp == 0) ? ((short)(((mant << 1) + 1) + sig)) : ((short)(((((int)1) << ((byte)(iexp - 1))) * ((mant << 1) + 0x21)) + sig));
                sss = (short)(ss / 0x1000);
                short ssm = (short)(ss & 0xfff);
                ssq = (short)(ssm << 1);
            }
            else
            {
                s1 = (short)(s1 ^ 0x80);
                if (s1 >= 0x80)
                {
                    s1 = (short)(s1 + -128);
                    s1 = (short)(s1 ^ 0x7f);
                    sig = 0x2000;
                }
                else
                {
                    sig = 0;
                    s1 = (short)(s1 ^ 0x7f);
                }
                iexp = s1 / 0x10;
                mant = s1 - (iexp << 4);
                ss = (iexp == 0) ? ((short)((mant << 1) + sig)) : ((short)((((((int)1) << ((byte)iexp)) * ((mant << 1) + 0x21)) - 0x21) + sig));
                sss = (short)(ss / 0x2000);
                ssq = (short)(ss & 0x1fff);
            }
            return ((sss == 0) ? ssq : ((short)((0x4000 - ssq) & 0x3fff)));
        }

        private short G726_filta(short fi, short dms)
        {
            short dif = (short)((((fi << 9) + 0x2000) - dms) & 0x1fff);
            short difsx = (((short)(dif >> 12)) == 0) ? ((short)(dif >> 5)) : ((short)((dif >> 5) + 0xf00));
            return (short)((difsx + dms) & 0xfff);
        }

        private short G726_filtb(short fi, short dml)
        {
            long fi1 = fi;
            long dml1 = dml;
            long dif = (((fi1 << 11) + 0x8000) - dml1) & 0x7fff;
            long difs = dif >> 14;
            long difsx = (difs == 0) ? (dif >> 7) : ((dif >> 7) + 0x3f00);
            return (short)((difsx + dml1) & 0x3fff);
        }

        private short G726_filtc(short ax, short ap)
        {
            short dif = (short)((((ax << 9) + 0x800) - ap) & 0x7ff);
            short difsx = (((short)(dif >> 10)) == 0) ? ((short)(dif >> 4)) : ((short)((dif >> 4) + 0x380));
            return (short)((difsx + ap) & 0x3ff);
        }

        private short G726_filtd(short wi, short y)
        {
            long wi1 = wi;
            long y1 = y;
            long dif = (((wi1 << 5) + 0x20000) - y1) & 0x1ffff;
            long difs = dif >> 0x10;
            long difsx = (difs == 0) ? (dif >> 5) : ((dif >> 5) + 0x1000);
            return (short)((y1 + difsx) & 0x1fff);
        }

        private long G726_filte(short yup, long yl)
        {
            long yup1 = yup;
            long dif1 = 0x100000 - yl;
            long dif = (yup1 + (dif1 >> 6)) & 0x3fff;
            long difs = dif >> 13;
            long difsx = (difs == 0) ? dif : (dif + 0x7c000);
            return ((yl + difsx) & 0x7ffff);
        }

        private short G726_floata(short dq)
        {
            long exp_;
            long dqs = (dq >> 15) & 1;
            long mag = dq & 0x7fff;
            if (mag >= 0x4000)
            {
                exp_ = 15;
            }
            else if (mag >= 0x2000)
            {
                exp_ = 14;
            }
            else if (mag >= 0x1000)
            {
                exp_ = 13;
            }
            else if (mag >= 0x800)
            {
                exp_ = 12;
            }
            else if (mag >= 0x400)
            {
                exp_ = 11;
            }
            else if (mag >= 0x200)
            {
                exp_ = 10;
            }
            else if (mag >= 0x100)
            {
                exp_ = 9;
            }
            else if (mag >= 0x80)
            {
                exp_ = 8;
            }
            else if (mag >= 0x40)
            {
                exp_ = 7;
            }
            else if (mag >= 0x20)
            {
                exp_ = 6;
            }
            else if (mag >= 0x10)
            {
                exp_ = 5;
            }
            else if (mag >= 8)
            {
                exp_ = 4;
            }
            else if (mag >= 4)
            {
                exp_ = 3;
            }
            else if (mag >= 2)
            {
                exp_ = 2;
            }
            else if (mag == 1)
            {
                exp_ = 1;
            }
            else
            {
                exp_ = 0;
            }
            long mant = (mag == 0) ? 0x20 : ((mag << 6) >> ((byte)exp_));
            return (short)(((dqs << 10) + (exp_ << 6)) + mant);
        }

        private short G726_floatb(short sr)
        {
            long exp_;
            long srr = sr & 0xffff;
            long srs = srr >> 15;
            long mag = (srs == 0) ? srr : ((0x10000 - srr) & 0x7fff);
            if (mag >= 0x4000)
            {
                exp_ = 15;
            }
            else if (mag >= 0x2000)
            {
                exp_ = 14;
            }
            else if (mag >= 0x1000)
            {
                exp_ = 13;
            }
            else if (mag >= 0x800)
            {
                exp_ = 12;
            }
            else if (mag >= 0x400)
            {
                exp_ = 11;
            }
            else if (mag >= 0x200)
            {
                exp_ = 10;
            }
            else if (mag >= 0x100)
            {
                exp_ = 9;
            }
            else if (mag >= 0x80)
            {
                exp_ = 8;
            }
            else if (mag >= 0x40)
            {
                exp_ = 7;
            }
            else if (mag >= 0x20)
            {
                exp_ = 6;
            }
            else if (mag >= 0x10)
            {
                exp_ = 5;
            }
            else if (mag >= 8)
            {
                exp_ = 4;
            }
            else if (mag >= 4)
            {
                exp_ = 3;
            }
            else if (mag >= 2)
            {
                exp_ = 2;
            }
            else if (mag == 1)
            {
                exp_ = 1;
            }
            else
            {
                exp_ = 0;
            }
            long mant = (mag == 0) ? 0x20 : ((mag << 6) >> ((byte)exp_));
            return (short)(((srs << 10) + (exp_ << 6)) + mant);
        }

        private short G726_fmult(short An, short SRn)
        {
            long anexp;
            long an = An & 0xffff;
            long srn1 = SRn & 0xffff;
            long ans = an & 0x8000;
            ans = ans >> 15;
            long anmag = (ans == 0) ? (an >> 2) : ((0x4000 - (an >> 2)) & 0x1fff);
            if (anmag >= 0x1000)
            {
                anexp = 13;
            }
            else if (anmag >= 0x800)
            {
                anexp = 12;
            }
            else if (anmag >= 0x400)
            {
                anexp = 11;
            }
            else if (anmag >= 0x200)
            {
                anexp = 10;
            }
            else if (anmag >= 0x100)
            {
                anexp = 9;
            }
            else if (anmag >= 0x80)
            {
                anexp = 8;
            }
            else if (anmag >= 0x40)
            {
                anexp = 7;
            }
            else if (anmag >= 0x20)
            {
                anexp = 6;
            }
            else if (anmag >= 0x10)
            {
                anexp = 5;
            }
            else if (anmag >= 8)
            {
                anexp = 4;
            }
            else if (anmag >= 4)
            {
                anexp = 3;
            }
            else if (anmag >= 2)
            {
                anexp = 2;
            }
            else if (anmag == 1)
            {
                anexp = 1;
            }
            else
            {
                anexp = 0;
            }
            long anmant = (anmag == 0) ? 0x20 : ((anmag << 6) >> ((byte)anexp));
            long srns = srn1 >> 10;
            long srnexp = (srn1 >> 6) & 15;
            long srnmant = srn1 & 0x3f;
            long wans = srns ^ ans;
            long wanexp = srnexp + anexp;
            long wanmant = ((srnmant * anmant) + 0x30) >> 4;
            long wanmag = (wanexp <= 0x1a) ? ((wanmant << 7) >> ((byte)(0x1a - wanexp))) : (((wanmant << 7) << ((byte)(wanexp - 0x1a))) & 0x7fff);
            long wan = (wans == 0) ? wanmag : ((0x10000 - wanmag) & 0xffff);
            return (short)wan;
        }

        private short G726_functf(short rate, short i)
        {
            short im;
            short[] tab;
            if (rate == 4)
            {
                tab = new short[] { 0, 0, 0, 1, 1, 1, 3, 7 };
                im = (((short)(i >> 3)) == 0) ? ((short)(i & 7)) : ((short)((15 - i) & 7));
                return tab[im];
            }
            if (rate == 3)
            {
                tab = new short[] { 0, 1, 2, 7 };
                im = (((short)(i >> 2)) == 0) ? ((short)(i & 3)) : ((short)((7 - i) & 3));
                return tab[im];
            }
            if (rate == 2)
            {
                short[] CS = new short[2];
                CS[1] = 7;
                tab = CS;
                im = (((short)(i >> 1)) == 0) ? ((short)(i & 1)) : ((short)((3 - i) & 1));
                return tab[im];
            }
            tab = new short[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 3, 4, 5, 6, 6 };
            im = (((short)(i >> 4)) == 0) ? ((short)(i & 15)) : ((short)((0x1f - i) & 15));
            return tab[im];
        }

        private short G726_functw(short rate, short i)
        {
            short[] tab;
            short im;
            if (rate == 4)
            {
                tab = new short[] { 0xff4, 0x12, 0x29, 0x40, 0x70, 0xc6, 0x163, 0x462 };
                im = (((short)(i >> 3)) == 0) ? ((short)(i & 7)) : ((short)((15 - i) & 7));
                return tab[im];
            }
            if (rate == 3)
            {
                tab = new short[] { 0xffc, 30, 0x89, 0x246 };
                im = (((short)(i >> 2)) == 0) ? ((short)(i & 3)) : ((short)((7 - i) & 3));
                return tab[im];
            }
            if (rate == 2)
            {
                tab = new short[] { 0xfea, 0x1b7 };
                im = (((short)(i >> 1)) == 0) ? ((short)(i & 1)) : ((short)((3 - i) & 1));
                return tab[im];
            }
            tab = new short[] { 14, 14, 0x18, 0x27, 40, 0x29, 0x3a, 100, 0x8d, 0xb3, 0xdb, 280, 0x166, 440, 0x211, 0x2b8 };
            im = (((short)(i >> 4)) == 0) ? ((short)(i & 15)) : ((short)((0x1f - i) & 15));
            return tab[im];
        }

        private short G726_lima(short ap)
        {
            return ((ap >= 0x100) ? ((short)0x40) : ((short)(ap >> 2)));
        }

        private short G726_limb(short yut)
        {
            short geul = (short)(((yut + 0x2c00) & 0x3fff) >> 13);
            short gell = (short)(((yut + 0x3de0) & 0x3fff) >> 13);
            if (gell == 1)
            {
                return 0x220;
            }
            if (geul == 0)
            {
                return 0x1400;
            }
            return yut;
        }

        private short G726_limc(short a2t)
        {
            long a2p1;
            long a2t1 = a2t & 0xffff;
            long a2ul = 0x3000;
            long a2ll = 0xd000;
            if ((a2t1 >= 0x8000) && (a2t1 <= a2ll))
            {
                a2p1 = a2ll;
            }
            else if ((a2t1 >= a2ul) && (a2t1 <= 0x7fff))
            {
                a2p1 = a2ul;
            }
            else
            {
                a2p1 = a2t1;
            }
            return (short)a2p1;
        }

        private short G726_limd(short a1t, short a2p)
        {
            long a1p1;
            long a1t1 = a1t & 0xffff;
            long a2p1 = a2p & 0xffff;
            long ome = 0x3c00;
            long a1ul = ((ome + 0x10000) - a2p1) & 0xffff;
            long a1ll = ((a2p1 + 0x10000) - ome) & 0xffff;
            if ((a1t1 >= 0x8000) && (a1t1 <= a1ll))
            {
                a1p1 = a1ll;
            }
            else if ((a1t1 >= a1ul) && (a1t1 <= 0x7fff))
            {
                a1p1 = a1ul;
            }
            else
            {
                a1p1 = a1t1;
            }
            return (short)a1p1;
        }

        private void G726_log(ref short d, ref short dl, ref short ds)
        {
            long exp_;
            ds = (short)(d >> 15);
            long d1 = (long)d;
            long dqm = (ds > 0) ? ((0x10000 - d1) & 0x7fff) : d1;
            if (dqm >= 0x4000)
            {
                exp_ = 14;
            }
            else if (dqm >= 0x2000)
            {
                exp_ = 13;
            }
            else if (dqm >= 0x1000)
            {
                exp_ = 12;
            }
            else if (dqm >= 0x800)
            {
                exp_ = 11;
            }
            else if (dqm >= 0x400)
            {
                exp_ = 10;
            }
            else if (dqm >= 0x200)
            {
                exp_ = 9;
            }
            else if (dqm >= 0x100)
            {
                exp_ = 8;
            }
            else if (dqm >= 0x80)
            {
                exp_ = 7;
            }
            else if (dqm >= 0x40)
            {
                exp_ = 6;
            }
            else if (dqm >= 0x20)
            {
                exp_ = 5;
            }
            else if (dqm >= 0x10)
            {
                exp_ = 4;
            }
            else if (dqm >= 8)
            {
                exp_ = 3;
            }
            else if (dqm >= 4)
            {
                exp_ = 2;
            }
            else if (dqm >= 2)
            {
                exp_ = 1;
            }
            else
            {
                exp_ = 0;
            }
            long mant = ((dqm << 7) >> ((byte)exp_)) & 0x7f;
            dl = (short)((exp_ << 7) + mant);
        }

        private short G726_mix(short al, short yu, long yl)
        {
            long al1 = al;
            long yu1 = yu;
            long dif = ((yu1 + 0x4000) - (yl >> 6)) & 0x3fff;
            long difs = dif >> 13;
            long difm = (difs == 0) ? dif : ((0x4000 - dif) & 0x1fff);
            long prodm = (difm * al1) >> 6;
            long prod = (difs == 0) ? prodm : ((0x4000 - prodm) & 0x3fff);
            return (short)(((yl >> 6) + prod) & 0x1fff);
        }

        private short G726_quan(short rate, short dln, short ds)
        {
            short i = 0;
            if (rate == 4)
            {
                if (dln >= 0xf84)
                {
                    i = 1;
                }
                else if (dln >= 0x800)
                {
                    i = 15;
                }
                else if (dln >= 400)
                {
                    i = 7;
                }
                else if (dln >= 0x15d)
                {
                    i = 6;
                }
                else if (dln >= 300)
                {
                    i = 5;
                }
                else if (dln >= 0xf6)
                {
                    i = 4;
                }
                else if (dln >= 0xb2)
                {
                    i = 3;
                }
                else if (dln >= 80)
                {
                    i = 2;
                }
                else
                {
                    i = 1;
                }
                if (ds > 0)
                {
                    i = (short)(15 - i);
                }
                if (i == 0)
                {
                    i = 15;
                }
                return i;
            }
            if (rate == 3)
            {
                if (dln >= 0x800)
                {
                    i = 7;
                }
                else if (dln >= 0x14b)
                {
                    i = 3;
                }
                else if (dln >= 0xda)
                {
                    i = 2;
                }
                else if (dln >= 8)
                {
                    i = 1;
                }
                else if (dln >= 0)
                {
                    i = 7;
                }
                if (ds > 0)
                {
                    i = (short)(7 - i);
                }
                if (i == 0)
                {
                    i = 7;
                }
                return i;
            }
            if (rate == 2)
            {
                if (dln >= 0x800)
                {
                    i = 0;
                }
                else if (dln >= 0x105)
                {
                    i = 1;
                }
                else
                {
                    i = 0;
                }
                if (ds > 0)
                {
                    i = (short)(3 - i);
                }
                return i;
            }
            if (dln >= 0xff0)
            {
                i = 2;
            }
            else if (dln >= 0xf86)
            {
                i = 1;
            }
            else if (dln >= 0x800)
            {
                i = 0x1f;
            }
            else if (dln >= 0x229)
            {
                i = 15;
            }
            else if (dln >= 0x210)
            {
                i = 14;
            }
            else if (dln >= 0x1f6)
            {
                i = 13;
            }
            else if (dln >= 0x1db)
            {
                i = 12;
            }
            else if (dln >= 0x1bd)
            {
                i = 11;
            }
            else if (dln >= 0x19d)
            {
                i = 10;
            }
            else if (dln >= 0x17a)
            {
                i = 9;
            }
            else if (dln >= 0x153)
            {
                i = 8;
            }
            else if (dln >= 0x12a)
            {
                i = 7;
            }
            else if (dln >= 250)
            {
                i = 6;
            }
            else if (dln >= 0xc6)
            {
                i = 5;
            }
            else if (dln >= 0x8b)
            {
                i = 4;
            }
            else if (dln >= 0x44)
            {
                i = 3;
            }
            else if (dln >= 0)
            {
                i = 2;
            }
            if (ds > 0)
            {
                i = (short)(0x1f - i);
            }
            if (i == 0)
            {
                i = 0x1f;
            }
            return i;
        }

        private void G726_reconst(short rate, ref short i, ref short dqln, ref short dqs)
        {
            short[] tab;
            if (rate == 4)
            {
                tab = new short[] { 0x800, 4, 0x87, 0xd5, 0x111, 0x143, 0x175, 0x1a9, 0x1a9, 0x175, 0x143, 0x111, 0xd5, 0x87, 4, 0x800 };
                dqs = (short)(i >> 3);
                dqln = tab[i];
            }
            else if (rate == 3)
            {
                tab = new short[] { 0x800, 0x87, 0x111, 0x175, 0x175, 0x111, 0x87, 0x800 };
                dqs = (short)(i >> 2);
                dqln = tab[i];
            }
            else if (rate == 2)
            {
                tab = new short[] { 0x74, 0x16d, 0x16d, 0x74 };
                dqs = (short)(i >> 1);
                dqln = tab[i];
            }
            else
            {
                tab = new short[] { 
                0x800, 0xfbe, 0x1c, 0x68, 0xa9, 0xe0, 0x112, 0x13e, 0x166, 0x18b, 0x1ad, 0x1cb, 0x1e8, 0x202, 0x21b, 0x236, 
                0x236, 0x21b, 0x202, 0x1e8, 0x1cb, 0x1ad, 0x18b, 0x166, 0x13e, 0x112, 0xe0, 0xa9, 0x68, 0x1c, 0xfbe, 0x800
             };
                dqs = (short)(i >> 4);
                dqln = tab[i];
            }
        }

        private short G726_subta(short sl, short se)
        {
            short sls = (short)(sl >> 13);
            long sl1 = sl;
            long se1 = se;
            long sli = (sls == 0) ? sl1 : (sl1 + 0xc000);
            long sei = (((short)(se >> 14)) == 0) ? se1 : (se1 + 0x8000);
            return (short)(((sli + 0x10000) - sei) & 0xffff);
        }

        private short G726_subtb(short dl, short y)
        {
            return (short)(((dl + 0x1000) - (y >> 2)) & 0xfff);
        }

        private short G726_subtc(short dmsp, short dmlp, short tdp, short y)
        {
            long dmsp1 = dmsp;
            long dmlp1 = dmlp;
            long dif = (((dmsp1 << 2) + 0x8000) - dmlp1) & 0x7fff;
            long difs = dif >> 14;
            long difm = (difs == 0) ? dif : ((0x8000 - dif) & 0x3fff);
            long dthr = dmlp1 >> 3;
            return ((((y >= 0x600) && (difm < dthr)) && (tdp == 0)) ? ((short)0) : ((short)1));
        }

        private short G726_sync(short rate, short i, short sp, short dlnx, short dsx, char law)
        {
            short id;
            short im;
            if (rate == 4)
            {
                im = (((short)(i >> 3)) == 0) ? ((short)(i + 8)) : ((short)(i & 7));
                if (dlnx >= 0xf84)
                {
                    id = 9;
                }
                else if (dlnx >= 0x800)
                {
                    id = 7;
                }
                else if (dlnx >= 400)
                {
                    id = 15;
                }
                else if (dlnx >= 0x15d)
                {
                    id = 14;
                }
                else if (dlnx >= 300)
                {
                    id = 13;
                }
                else if (dlnx >= 0xf6)
                {
                    id = 12;
                }
                else if (dlnx >= 0xb2)
                {
                    id = 11;
                }
                else if (dlnx >= 80)
                {
                    id = 10;
                }
                else
                {
                    id = 9;
                }
                if (dsx > 0)
                {
                    id = (short)(15 - id);
                }
                if (id == 8)
                {
                    id = 7;
                }
            }
            else if (rate == 3)
            {
                im = (((short)(i >> 2)) == 0) ? ((short)(i + 4)) : ((short)(i & 3));
                if (dlnx >= 0x800)
                {
                    id = 3;
                }
                else if (dlnx >= 0x14b)
                {
                    id = 7;
                }
                else if (dlnx >= 0xda)
                {
                    id = 6;
                }
                else if (dlnx >= 8)
                {
                    id = 5;
                }
                else if (dlnx >= 0)
                {
                    id = 3;
                }
                else
                {
                    id = 0;
                }
                if (dsx > 0)
                {
                    id = (short)(7 - id);
                }
                if (id == 4)
                {
                    id = 3;
                }
            }
            else if (rate == 2)
            {
                im = (((short)(i >> 1)) == 0) ? ((short)(i + 2)) : ((short)(i & 1));
                if (dlnx >= 0x800)
                {
                    id = 2;
                }
                else if (dlnx >= 0x105)
                {
                    id = 3;
                }
                else if (dlnx >= 0)
                {
                    id = 2;
                }
                else
                {
                    id = 0;
                }
                if (dsx > 0)
                {
                    id = (short)(3 - id);
                }
            }
            else
            {
                im = (((short)(i >> 4)) == 0) ? ((short)(i + 0x10)) : ((short)(i & 15));
                if (dlnx >= 0xff0)
                {
                    id = 0x12;
                }
                else if (dlnx >= 0xf86)
                {
                    id = 0x11;
                }
                else if (dlnx >= 0x800)
                {
                    id = 15;
                }
                else if (dlnx >= 0x229)
                {
                    id = 0x1f;
                }
                else if (dlnx >= 0x210)
                {
                    id = 30;
                }
                else if (dlnx >= 0x1f6)
                {
                    id = 0x1d;
                }
                else if (dlnx >= 0x1db)
                {
                    id = 0x1c;
                }
                else if (dlnx >= 0x1bd)
                {
                    id = 0x1b;
                }
                else if (dlnx >= 0x19d)
                {
                    id = 0x1a;
                }
                else if (dlnx >= 0x17a)
                {
                    id = 0x19;
                }
                else if (dlnx >= 0x153)
                {
                    id = 0x18;
                }
                else if (dlnx >= 0x12a)
                {
                    id = 0x17;
                }
                else if (dlnx >= 250)
                {
                    id = 0x16;
                }
                else if (dlnx >= 0xc6)
                {
                    id = 0x15;
                }
                else if (dlnx >= 0x8b)
                {
                    id = 20;
                }
                else if (dlnx >= 0x44)
                {
                    id = 0x13;
                }
                else if (dlnx >= 0)
                {
                    id = 0x12;
                }
                else
                {
                    id = 0;
                }
                if (dsx > 0)
                {
                    id = (short)(0x1f - id);
                }
                if (id == 0x10)
                {
                    id = 15;
                }
            }
            short ss = (short)((sp & 0x80) >> 7);
            short mask = (short)(sp & 0x7f);
            if (law == '1')
            {
                if (((id > im) && (ss == 1)) && (mask == 0))
                {
                    ss = 0;
                }
                else if (((id > im) && (ss == 1)) && (mask != 0))
                {
                    mask = (short)(mask - 1);
                }
                else if (((id > im) && (ss == 0)) && (mask != 0x7f))
                {
                    mask = (short)(mask + 1);
                }
                else if (((id < im) && (ss == 1)) && (mask != 0x7f))
                {
                    mask = (short)(mask + 1);
                }
                else if (((id < im) && (ss == 0)) && (mask == 0))
                {
                    ss = 1;
                }
                else if (((id < im) && (ss == 0)) && (mask != 0))
                {
                    mask = (short)(mask - 1);
                }
            }
            else if (((id > im) && (ss == 1)) && (mask == 0x7f))
            {
                ss = 0;
                mask = (short)(mask - 1);
            }
            else if (((id > im) && (ss == 1)) && (mask != 0x7f))
            {
                mask = (short)(mask + 1);
            }
            else if (((id > im) && (ss == 0)) && (mask != 0))
            {
                mask = (short)(mask - 1);
            }
            else if (((id < im) && (ss == 1)) && (mask != 0))
            {
                mask = (short)(mask - 1);
            }
            else if (((id < im) && (ss == 0)) && (mask == 0x7f))
            {
                ss = 1;
            }
            else if (((id < im) && (ss == 0)) && (mask != 0x7f))
            {
                mask = (short)(mask + 1);
            }
            return (short)(mask + (ss << 7));
        }

        private short G726_tone(short a2p)
        {
            long a2p1 = a2p & 0xffff;
            return (((a2p1 >= 0x8000) && (a2p1 < 0xd200)) ? ((short)1) : ((short)0));
        }

        private short G726_trans(short td, long yl, short dq)
        {
            short dqmag = (short)(dq & 0x7fff);
            short ylint = (short)(yl >> 15);
            short ylfrac = (short)((yl >> 10) & 0x1f);
            long thr1 = (ylfrac + 0x20) << ylint;
            long thr2 = (ylint > 9) ? 0x7c00 : thr1;
            long dqthr = (thr2 + (thr2 >> 1)) >> 1;
            long dqmag1 = dqmag;
            return (((dqmag1 > dqthr) && (td == 1)) ? ((short)1) : ((short)0));
        }

        private short G726_triga(short tr, short app)
        {
            return ((tr == 0) ? app : ((short)0x100));
        }

        private short G726_trigb(short tr, short ap)
        {
            return ((tr == 0) ? ap : ((short)0));
        }

        private short G726_upa1(short pk0, short pk1, short a1, short sigpk)
        {
            long a11 = a1 & 0xffff;
            short pks = (short)(pk0 ^ pk1);
            long uga1 = (sigpk == 1) ? ((long)0) : ((pks == 0) ? ((long)0xc0) : ((long)0xff40));
            long a1s = a11 >> 15;
            long ash = a11 >> 8;
            long ula1 = ((a1s == 0) ? (0x10000 - ash) : (0x10000 - (ash + 0xff00))) & 0xffff;
            long ua1 = (uga1 + ula1) & 0xffff;
            return (short)((a11 + ua1) & 0xffff);
        }

        private short G726_upa2(short pk0, short pk1, short pk2, short a2, short a1, short sigpk)
        {
            long fa1;
            long a11 = a1 & 0xffff;
            long a21 = a2 & 0xffff;
            short pks1 = (short)(pk0 ^ pk1);
            long uga2a = (((short)(pk0 ^ pk2)) == 0) ? ((long)0x4000) : ((long)0x1c000);
            short a1s = (short)(a1 >> 15);
            if (a1s == 0)
            {
                fa1 = (a11 <= 0x1fff) ? (a11 << 2) : 0x7ffc;
            }
            else
            {
                fa1 = (a11 >= 0xe001) ? ((a11 << 2) & 0x1ffff) : 0x18004;
            }
            long fa = (pks1 > 0) ? fa1 : ((0x20000 - fa1) & 0x1ffff);
            long uga2b = (uga2a + fa) & 0x1ffff;
            long uga2s = uga2b >> 0x10;
            long uga2 = (sigpk == 1) ? 0 : ((uga2s > 0) ? ((uga2b >> 7) + 0xfc00) : (uga2b >> 7));
            long ula2 = (((short)(a2 >> 15)) == 0) ? ((0x10000 - (a21 >> 7)) & 0xffff) : ((0x10000 - ((a21 >> 7) + 0xfe00)) & 0xffff);
            long ua2 = (uga2 + ula2) & 0xffff;
            return (short)((a21 + ua2) & 0xffff);
        }

        private short G726_upb(short rate, short u, short b, short dq)
        {
            ushort param;
            ushort leak;
            long bb = b & 0xffff;
            short dqmag = (short)(dq & 0x7fff);
            if (rate != 5)
            {
                leak = 8;
                param = 0xff00;
            }
            else
            {
                leak = 9;
                param = 0xff80;
            }
            long ugb = (dqmag == 0) ? ((long)0) : ((u == 0) ? ((long)0x80) : ((long)0xff80));
            long bs = bb >> 15;
            long ulb = (bs == 0) ? ((0x10000 - (bb >> leak)) & 0xffff) : ((0x10000 - ((bb >> leak) + param)) & 0xffff);
            long ub = (ugb + ulb) & 0xffff;
            return (short)((bb + ub) & 0xffff);
        }

        private short G726_xor(short dqn, short dq)
        {
            short dqs = (short)((dq >> 15) & 1);
            short dqns = (short)(dqn >> 10);
            return (short)(dqs ^ dqns);
        }

        public byte[] G726ToG711Decode(byte[] linbuffer, int length)
        {
            int i;
            G726_STATE states = new G726_STATE();
            short[] shortarray1 = new short[length * 2];
            for (i = 0; i < length; i++)
            {
                shortarray1[2 * i] = (short)(linbuffer[i] & 15);
                shortarray1[(2 * i) + 1] = (short)((linbuffer[i] & 240) >> 4);
            }
            short[] shortarray2 = this.G726_decode(shortarray1, (long)(length * 2), '1', 4, 1, states);
            byte[] outbuffer = new byte[length * 2];
            for (i = 0; i < (length * 2); i++)
            {
                outbuffer[i] = (byte)shortarray2[i];
            }
            return outbuffer;
        }

        public byte[] G726ToPCMDecode(byte[] linbuffer, int length)
        {
            int i;
            G726_STATE states = new G726_STATE();
            short[] shortarray1 = new short[length * 2];
            for (i = 0; i < length; i++)
            {
                shortarray1[2 * i] = (short)(linbuffer[i] & 15);
                shortarray1[(2 * i) + 1] = (short)((linbuffer[i] & 240) >> 4);
            }
            short[] shortarray2 = this.G726_decode(shortarray1, (long)(length * 2), '1', 4, 1, states);
            byte[] tmparray1 = new byte[length * 2];
            for (i = 0; i < (length * 2); i++)
            {
                tmparray1[i] = (byte)shortarray2[i];
            }
            int len1 = length * 2;
            short[] shortarray3 = this.Alaw_Expand(tmparray1, (long)len1);
            byte[] outbuffer = new byte[len1 * 2];
            for (i = 0; i < len1; i++)
            {
                byte[] tmp1 = BitConverter.GetBytes(shortarray3[i]);
                outbuffer[i * 2] = tmp1[0];
                outbuffer[(i * 2) + 1] = tmp1[1];
            }
            return outbuffer;
        }

        public byte[] PCMToG711Encode(byte[] linbuffer, int length)
        {
            short[] smpbyte = new short[length / 2];
            for (int i = 0; i < (length / 2); i++)
            {
                smpbyte[i] = BitConverter.ToInt16(linbuffer, 2 * i);
            }
            return this.Alaw_Compress(smpbyte, (long)smpbyte.Length);
        }

        public byte[] PCMToG726Encode(byte[] linbuffer, int length)
        {
            int i;
            short[] shortarray1 = new short[length / 2];
            for (i = 0; i < (length / 2); i++)
            {
                shortarray1[i] = BitConverter.ToInt16(linbuffer, 2 * i);
            }
            byte[] tmparray1 = this.Alaw_Compress(shortarray1, (long)shortarray1.Length);
            int len1 = length / 2;
            G726_STATE states = new G726_STATE();
            short[] shortarray2 = new short[len1];
            for (i = 0; i < len1; i++)
            {
                shortarray2[i] = tmparray1[i];
            }
            short[] shortarray3 = this.G726_encode(shortarray2, (long)len1, '1', 4, 1, states);
            byte[] outbuffer = new byte[len1 / 2];
            for (i = 0; i < (len1 / 2); i++)
            {
                outbuffer[i] = (byte)((shortarray3[2 * i] & 15) + ((shortarray3[(2 * i) + 1] & 15) << 4));
            }
            return outbuffer;
        }

        public short[] ulaw_compress(short[] inbuf, long lenth)
        {
            short[] outbuf = new short[lenth];
            for (long n = 0; n < lenth; n++)
            {
                short absno = (inbuf[(int)((IntPtr)n)] < 0) ? ((short)((~inbuf[(int)((IntPtr)n)] >> 2) + 0x21)) : ((short)((inbuf[(int)((IntPtr)n)] >> 2) + 0x21));
                if (absno > 0x1fff)
                {
                    absno = 0x1fff;
                }
                short i = (short)(absno >> 6);
                short segno = 1;
                while (i != 0)
                {
                    segno = (short)(segno + 1);
                    i = (short)(i >> 1);
                }
                short high_nibble = (short)(8 - segno);
                short low_nibble = (short)((absno >> segno) & 15);
                low_nibble = (short)(15 - low_nibble);
                outbuf[(int)((IntPtr)n)] = (short)((short)(high_nibble << 4) | low_nibble);
                if (inbuf[(int)((IntPtr)n)] >= 0)
                {
                    outbuf[(int)((IntPtr)n)] = (short)(outbuf[(int)((IntPtr)n)] | 0x80);
                }
            }
            return outbuf;
        }

        public short[] ulaw_expand(short[] inbuf, long lenth)
        {
            short[] outbuf = new short[lenth];
            for (long n = 0; n < lenth; n++)
            {
                short sign = (inbuf[(int)((IntPtr)n)] < 0x80) ? ((short)(-1)) : ((short)1);
                short mantissa = (short)(~inbuf[(int)((IntPtr)n)]);
                short exponent = (short)((mantissa >> 4) & 7);
                short segment = (short)(exponent + 1);
                mantissa = (short)(mantissa & 15);
                short step = (short)(((int)4) << segment);
                outbuf[(int)((IntPtr)n)] = (short)(sign * ((((((int)0x80) << exponent) + (step * mantissa)) + (step / 2)) - 0x84));
            }
            return outbuf;
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct G726_STATE
        {
            public short sr0;
            public short sr1;
            public short a1r;
            public short a2r;
            public short b1r;
            public short b2r;
            public short b3r;
            public short b4r;
            public short b5r;
            public short b6r;
            public short dq5;
            public short dq4;
            public short dq3;
            public short dq2;
            public short dq1;
            public short dq0;
            public short dmsp;
            public short dmlp;
            public short apr;
            public short yup;
            public short tdr;
            public short pk0;
            public short pk1;
            public long ylp;
        }
    }


}
