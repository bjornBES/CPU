# CPUSpecs
General Information
8 bit data width
16 bit address width
64 kb of RAM and ROM

## About the CPU
At the Start the CPU executes the [start program](CPUSpecs.md###StartProgram)
CPUs PC starts at addr FFF7 after that it jumps to the starting ADDR at ADDR 0xFFFE def 0x0000

in the Reset thing it resets the console and the curor pos in mem

### StartProgram
```
PC = 0xFFF7
STOI #01 &FE04
STOI #00 &FE00
STOI #00 &FE01
UINT #00		    ; for updating the cursor
JUMP &0000      ; This address is where the CPU jumps to, to start the program
```

## Addressing vectors
Interrupt address vector is at 0xFF00 and 0xFF01

Starting address vector at 0xFFFE and 0xFFFF

## CPU registers
A 8 bit general purpose register

B 8 bit general purpose register

C 8 bit IO register

D 8 bit general purpose register

## ADDR types
### Absolute addressing 
here the address is in the instruction itself where the instruction comes first and then the address/immediate

ZPa	| ZIM	1 byte + REG	&XX, %RR


## FLAGS
```
ZERO : 0	| is 1 if the last * instr was zero
TRUE : 1	| is 1 if the last CMP instr was the same
LESS : 2	| is 1 if the last CMP instr was less then
OVER : 3	| is 1 if the last * instr was over 255
INTS : 4	| is 1 if there is an INT
PLUS : 5	| is 1 if the last * instr was under 0
CARR : 6	| is 1 if the last * overflowed 255(0xFF)
```

## Status Reg
RUNNING : 0;
KEYHERE : 1;

## [Instructions](INSTR.txt)
there is op to 256 instructions on the instruction set from hex 0 to hex FF
all instructions can be found [here](INSTR.txt)

## PORTS
there is an IO port
this is used for printing something to the screen
see more [here](Ports.txt)

## MEM Layout
```
0x0000 - 0x00FF STACK
0x0100 - 0x01FF SOUND DATA if 0x7100 = 0 else GP ram ; WIP
0x0200 - 0x0FFF GP ram
0x1000 - 0x10FF assembler/compiler variables
0x1100 - 0x70FF GP ram
0x7100 - 0x7100 Sound address 
0x7101 - 0xFAFF GP ram
0xFB00 - 0xFB01 subroutine address readonly
0xFB02 - 0xFBFF GP ram
0xFC00 - 0xFC00 Stack pointer
0xFC01 - 0xFDFF GP ram
0xFE00 - 0xFE00 Cursor X position
0xFE01 - 0xFE01 Cursor Y position
0xFE02 - 0xFE02 Cursor Styles
0xFE03 - 0xFE03 Console background color
0xFE04 - 0xFE04 Console Clear
0xFE05 - 0xFE05 Get Key if 1 UINT 1 operation will work
0xFE06 - 0xFEFF GP ram
0xFF00 - 0xFF01 interrupt address
0xFF02 - 0xFFFD GP ram
0xFFFE - 0xFFFF Reset vector
```


