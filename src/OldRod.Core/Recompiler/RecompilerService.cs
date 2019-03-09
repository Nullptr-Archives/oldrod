// Project OldRod - A KoiVM devirtualisation utility.
// Copyright (C) 2019 Washi
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using AsmResolver.Net.Cil;
using OldRod.Core.Architecture;
using OldRod.Core.Recompiler.ILTranslation;
using OldRod.Core.Recompiler.VCallTranslation;

namespace OldRod.Core.Recompiler
{
    public static class RecompilerService
    {
        private static readonly IDictionary<ILCode, IOpCodeRecompiler> OpCodeRecompilers =
            new Dictionary<ILCode, IOpCodeRecompiler>();
        
        private static readonly IDictionary<VMCalls, IVCallRecompiler> VCallRecompilers =
            new Dictionary<VMCalls, IVCallRecompiler>();

        static RecompilerService()
        {
            SetupOpCodeRecompilers();
            SetupVCallRecompilers();
        }

        private static void SetupOpCodeRecompilers()
        {
            // Push
            var push = new PushRecompiler();
            OpCodeRecompilers[ILCode.PUSHR_BYTE] = push;
            OpCodeRecompilers[ILCode.PUSHR_WORD] = push;
            OpCodeRecompilers[ILCode.PUSHR_DWORD] = push;
            OpCodeRecompilers[ILCode.PUSHR_QWORD] = push;
            OpCodeRecompilers[ILCode.PUSHR_OBJECT] = push;
            OpCodeRecompilers[ILCode.PUSHI_DWORD] = push;
            OpCodeRecompilers[ILCode.PUSHI_QWORD] = push;

            // Pop
            OpCodeRecompilers[ILCode.POP] = new PopRecompiler();

            // Add
            var add = new SimpleOpCodeRecompiler(CilOpCodes.Add,
                ILCode.ADD_DWORD, ILCode.ADD_QWORD, ILCode.ADD_R32, ILCode.ADD_R64);
            OpCodeRecompilers[ILCode.ADD_DWORD] = add;
            OpCodeRecompilers[ILCode.ADD_QWORD] = add;
            OpCodeRecompilers[ILCode.ADD_R32] = add;
            OpCodeRecompilers[ILCode.ADD_R64] = add;

            // Cmp
            var cmp = new CmpRecompiler();
            OpCodeRecompilers[ILCode.CMP] = cmp;
            OpCodeRecompilers[ILCode.CMP_DWORD] = cmp;
            OpCodeRecompilers[ILCode.CMP_QWORD] = cmp;
            OpCodeRecompilers[ILCode.CMP_R32] = cmp;
            OpCodeRecompilers[ILCode.CMP_R64] = cmp;

            // Nor
            var nor = new SimpleOpCodeRecompiler(new[] { CilOpCodes.Or, CilOpCodes.Not, }, 
                ILCode.NOR_DWORD, ILCode.NOR_QWORD);
            OpCodeRecompilers[ILCode.NOR_DWORD] = nor;
            OpCodeRecompilers[ILCode.NOR_QWORD] = nor;

            // Sub
            OpCodeRecompilers[ILCode.__SUB_DWORD] = new SimpleOpCodeRecompiler(CilOpCodes.Sub, ILCode.__SUB_DWORD);
            
            // Or
            OpCodeRecompilers[ILCode.__OR_DWORD] = new SimpleOpCodeRecompiler(CilOpCodes.Or, ILCode.__OR_DWORD);

            // And
            OpCodeRecompilers[ILCode.__AND_DWORD] = new SimpleOpCodeRecompiler(CilOpCodes.And, ILCode.__AND_DWORD);

            // Xor
            OpCodeRecompilers[ILCode.__XOR_DWORD] = new SimpleOpCodeRecompiler(CilOpCodes.Xor, ILCode.__XOR_DWORD);
            
            // Not
            OpCodeRecompilers[ILCode.__NOT_DWORD] = new SimpleOpCodeRecompiler(CilOpCodes.Not, ILCode.__NOT_DWORD);
        }

        private static void SetupVCallRecompilers()
        {
            VCallRecompilers[VMCalls.BOX] = new BoxRecompiler();
            VCallRecompilers[VMCalls.ECALL] = new ECallRecompiler();
            VCallRecompilers[VMCalls.LDFLD] = new LdfldRecompiler();
            VCallRecompilers[VMCalls.STFLD] = new StfldRecompiler();
        }

        public static IOpCodeRecompiler GetOpCodeRecompiler(ILCode code)
        {
            if (!OpCodeRecompilers.TryGetValue(code, out var recompiler))
                    throw new NotSupportedException($"Recompilation of opcode {code} is not supported.");
            return recompiler;
        }

        public static IVCallRecompiler GetVCallRecompiler(VMCalls call)
        {
            if (!VCallRecompilers.TryGetValue(call, out var recompiler))
                throw new NotSupportedException($"Recompilation of vcall {call} is not supported.");
            return recompiler;
        }
    }
}