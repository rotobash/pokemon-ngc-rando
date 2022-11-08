using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public enum Registers
    {
        R0,
        R1,
        R2,
        R3,
        R4,
        R5,
        R6,
        R7,
        R8,
        R9,
        R10,
        R11,
        R12,
        R13,
        R14,
        R15,
        R16,
        R17,
        R18,
        R19,
        R20,
        R21,
        R22,
        R23,
        R24,
        R25,
        R26,
        R27,
        R28,
        R29,
        R30,
        R31,
        SP,
        // special purpose registers
        LR,
        CTR,
        SRR0,
        SRR1,
    }

    public enum ASMInstructionTypes
    {
        mr,
        mr_,

        mfspr,
        mflr,
        mfctr,
        mtspr,
        mtlr,

        mtctr,
        bctr,

        cmpw,
        cmplw,
        cmpwi,
        cmplwi,

        li,
        lis,

        extsb,
        extsh,

        slw,
        srawi,
        rlwinm,
        rlwinm_,

        lha,
        lhax,

        lbz,
        lhz,
        lwz,

        lbzx,
        lhzx,
        lwzx,

        stb,
        sth,
        stw,
        stwu,

        stbx,
        sthx,
        stwx,

        lmw,
        stmw,

        add,
        addi,
        addis,
        addze,

        sub,
        subi,
        neg,

        mulli,
        mullw,
        divw,
        divwu,

        or,
        ori,
        and,
        and_,
        andi_,

        b,
        bl,
        blr,

        beq,
        bne,
        blt,
        ble,
        bgt,
        bge,

        nop,

        // convenience instructions - not real PPC instructions
        // from offset to offset
        b_f,
        bl_f,
        beq_f,
        bne_f,
        blt_f,
        ble_f,
        bgt_f,
        bge_f,
        // to label with name
        b_l,
        bl_l,
        beq_l,
        bne_l,
        blt_l,
        ble_l,
        bgt_l,
        bge_l,

        // raw binary
        raw,

        // labels for conviently specifying addresses
        label,
    }

    public class ASM
    {
        public List<uint> Instructions { get; }
        public ASM()
        {
            Instructions = new List<uint>();
        }

        public void AddInstruction(ASMInstructionTypes instruction)
        {

        }
        
        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register)
        {

        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register1, Registers register2)
        {

        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register1, Registers register2, Registers register3)
        {
            var code = instruction switch
            {
                ASMInstructionTypes.add => (31 << 26)
            };
        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register, int immediate)
        {

        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register1, Registers register2, int immediate)
        {

        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register, uint immediate)
        {

        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register1, Registers register2, uint immediate)
        {

        }

        public void AddRegisterInstruction(ASMInstructionTypes instruction, Registers register, uint immediate1, uint immediate2, uint immediate3)
        {

        }
    }

    public static class ASMInstructions
    {
    }
}
