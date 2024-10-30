Hello TA :D


Exercise 8.1:
(i) Udført ved at følge instruks B i denne README.
(ii) Write up the bytecode in a more structured way with labels only at the beginning of the line (as in this chapter):
** ex3 **
Kør:
   open ParseAndComp;;
   compileToFile (fromFile "ex3.c") "ex3.out";; 

Write up bytecode in more structured way:
[  LDARGS;                       - Loads n (the CLI argument) on the stack
   CALL (1, "L1"); STOP;         - Pushes the current bp and return address (before the loaded args)

   Label "L1";                   - Main
   INCSP 1;                      - Dec(TypI, ”i”), allocate i
   GETBP; CSTI 1; ADD;           - AccVar ”i”, get address of i at offset 1
   CSTI 0;                       - CstI 0, put 0 on the stack
   STI;                          - Store 0 in address i
   INCSP -1;                     - Remove result of assigment (Expr)
   GOTO "L3";                    - Jump to condition check

   Label "L2";                   - Block
   GETBP; CSTI 1; ADD;           - AccVar ”i”, get address of i at offset 1
   LDI;                          - Access ”i”, load content of i
   PRINTI;                       - Print value of i
   INCSP -1;                     - Remove result of print (Expr)         
   GETBP; CSTI 1; ADD;           - AccVar ”i”, get address of i at offset 1
   GETBP; CSTI 1; ADD;           - AccVar ”i”, get address of i at offset 1
   LDI;                          - Load content of i
   CSTI 1;                       - CstI 1, put 1 on the stack
   ADD;                          - Increment i by 1
   STI;                          - Store incremented value back to i
   INCSP -1;                     - Remove result of assignment (Expr)
   INCSP 0; 
   
   Label "L3";                   - Condition check
   GETBP; CSTI 1; ADD;           - AccVar "i", get address of i at offset 1
   LDI;                          - Load content of i
   GETBP; CSTI 0; ADD;           - AccVar "n", get address of n at offset 0
   LDI;                          - Load content of n
   LT;                           - Compare i < n (LT = less than)
   IFNZRO "L2";                  - If not zero, go to label L2
   INCSP -1;                     - Remove result of comparison (Expr)
   RET 0                         - Return from function
]

With MicroC code side by side:
   LDARGS; CALL (1, "L1"); STOP;                -> void main(int n)
   Label "L1"; INCSP 1;                         -> int = i;
   GETBP; CSTI 1; ADD; CSTI 0; STI; INCSP -1;   -> i = 0;
   GOTO "L3";                                   -> while

   Label "L3"; GETBP; CSTI 1; ADD; LDI; GETBP; CSTI 0; ADD; LDI; LT; IFNZRO "L2"; INCSP -1;  -> (i < n)

   Label "L2"; GETBP; CSTI 1; ADD;  LDI; PRINTI;INCSP -1;                                    -> print i;                   
   GETBP; CSTI 1; ADD; GETBP; CSTI 1; ADD; LDI; CSTI 1; ADD; STI; INCSP -1; INCSP 0;         -> i=i+1


Prøve gennemgang af stacken for ex3.c hvor n=1:

**(Note:** &bp is just wierd notation for the current base pointer, to make it more visble that we are working with an address)

[] - LDARGS n (loader CLI argumentet)
[n] - Call (1, "L1")
[r|bp|n] - INCSP 1 (gør plads til ny variabel i)
[r|bp|n|0] - GETBP  (get address of bp on stack)
[r|bp|n|0|&bp] - CSTI 1
[r|bp|n|0|&bp|1] - ADD
[r|bp|n|0|&bp+1] - CSTI 0
[r|bp|n|0|&bp+1|0] - STI (stores 0 on &bp + offset 1 which is address for i), then removes the address from the stack
[r|bp|n|0|0] - INCSP -1 (STI does not remove the value it stored, after storing it, so this cleans it up)
[r|bp|n|0] - goto (ingen ændring) (jump til condition check "L3")
*vi har nu n og i på stakken og bp peger på n*

*L3*
[r|bp|n|0] - GETBP;CSTI 1;
[r|bp|n|0|&bp|1] - ADD
[r|bp|n|0|&bp+1] - LDI (load value på addressen af i)
[r|bp|n|0|0] - GETBP;CSTI 0;
[r|bp|n|0|0|&bp|0] - ADD
[r|bp|n|0|0|&bp+0] - LDI (load value på addressen af n)
[r|bp|n|0|0|n] - LT (0 < n) hvilket er true så resultatet er 1
[r|bp|n|0|1] - IFNZRO (jump til block "L2")
[r|bp|n|0]

*L2*
[r|bp|n|0] - GETBP;CSTI 1;
[r|bp|n|0|&bp|1] - ADD 
[r|bp|n|0|&bp+1] - LDI (load value på addressen af i)
[r|bp|n|0|0] - PRINTI
[r|bp|n|0|0] - INCSP -1 (cleanup af loaded value som blev brugt til print)
[r|bp|n|0] - GETBP;CSTI 1;
[r|bp|n|0|&bp|1] - ADD
[r|bp|n|0|&bp+1] - GETBP;CSTI 1
[r|bp|n|0|&bp+1|&bp|1] - ADD
[r|bp|n|0|&bp+1|&bp+1] - LDI (load value på addressen af i)
[r|bp|n|0|&bp+1|0] - CSTI 1
[r|bp|n|0|&bp+1|0|1] - ADD
[r|bp|n|0|&bp+1|1] - STI (stores 1 on bp + offset 1 which is address for i, then removes the address from the stack)
[r|bp|n|1|1] - INCSP -1 (cleanup af loaded value som blev stored på adressen for i)
[r|bp|n|1] - INCSP 0 (gør ingenting? koden er ikke optimeret)
[r|bp|n|1]

*L3*
[r|bp|n|1] - GETBP;CSTI 1;
[r|bp|n|1|bp|1] - ADD
[r|bp|n|1|bp+1] - LDI (load value på addressen af i)
[r|bp|n|1|1] - GETBP;CSTI 0;
[r|bp|n|1|1|bp|0] - ADD
[r|bp|n|1|1|bp+0] - LDI (load value på addressen af n)
[r|bp|n|1|1|n] - LT (1 < n) hvilket er false da n = 1 så resultatet er 0
[r|bp|n|1|0] - IFNZRO (jumper ikke)
[r|bp|n|1] - INCSP -1 (fjerner i som er en lokal variabel)
[r|bp|n] - RET 0
[n] - stop

**ex5** 
Kør:
   open ParseAndComp;;
   compileToFile (fromFile "ex5.c") "ex5.out";; 

og formatér og skriv coden side by side:
LDARGS; CALL (1, "L1"); STOP; Label "L1";                            -> void main(int n){
INCSP 1;                                                             -> int = r;
GETBP; CSTI 1; ADD; GETBP; CSTI 0; ADD; LDI; STI; INCSP -1;          -> r = n;
INCSP 1;                                                             -> int = r;
GETBP; CSTI 0; ADD; LDI; GETBP; CSTI 2; ADD; CALL (2, "L2");INCSP -1;-> square(n, &r);
GETBP; CSTI 2; ADD; LDI; PRINTI; INCSP -1; INCSP -1;                 -> print r;
GETBP; CSTI 1; ADD; LDI; PRINTI; INCSP -1; INCSP -1;                 -> print r;
RET 0;                                                               -> }
Label "L2";                                                          -> void square(int i, int *rp) {
GETBP; CSTI 1; ADD; LDI; GETBP; CSTI 0; ADD; LDI; GETBP; CSTI 0; ADD; LDI; MUL; STI; INCSP -1; INCSP 0; -> *rp = i * i;
RET 1;                                                               -> }


## Stacktrace:

**note:** &<var> (e.g. &bp) is weird notation for the current base pointer to make it more visible that we are working with an address. 
```bash
[ ]{0: LDARGS}
[ 4 ]{1: CALL 1 5} # main

[ 4 -999 4 ]{5: INCSP 1} # Allocate new var 'r'
[ 4 -999 4 0 ]{7: GETBP} # Get base pointer
[ 4 -999 4 0 2 ]{8: CSTI 1} # Push 1 to the stack
[ 4 -999 4 0 2 1 ]{10: ADD} # Add bp + offset(1) (address for r)
[ 4 -999 4 0 3 ]{11: GETBP} # Get base pointer
[ 4 -999 4 0 3 2 ]{12: CSTI 0} # Push 0 to the stack
[ 4 -999 4 0 3 2 0 ]{14: ADD} # Add &bp + offset(0) (address for n)
[ 4 -999 4 0 3 2 ]{15: LDI} # Load value of n
[ 4 -999 4 0 3 4 ]{16: STI} # Stores value of n on &r
[ 4 -999 4 4 4 ]{17: INCSP -1} # Shrink stack
[ 4 -999 4 4 ]{19: INCSP 1} # Allocate for new var r
[ 4 -999 4 4 4 ]{21: GETBP} # get base pointer
[ 4 -999 4 4 4 2 ]{22: CSTI 0} # push 0 to stack
[ 4 -999 4 4 4 2 0 ]{24: ADD} # Add &bp + offset(0)
[ 4 -999 4 4 4 2 ]{25: LDI} # Load value of n
[ 4 -999 4 4 4 4 ]{26: GETBP} # Get bp
[ 4 -999 4 4 4 4 2 ]{27: CSTI 2} # Push 2 
[ 4 -999 4 4 4 4 2 2 ]{29: ADD} # &bp + 2 (address of second r)
[ 4 -999 4 4 4 4 4 ]{30: CALL 2 57} # Call square with the 2 args on top of stack (n, adress of second r)
[ 4 -999 4 4 4 33 2 4 4 ]{57: GETBP} # Get bp
[ 4 -999 4 4 4 33 2 4 4 7 ]{58: CSTI 1} # Push 1 to stack
[ 4 -999 4 4 4 33 2 4 4 7 1 ]{60: ADD} # Add &bp + 1 (address of first r)
[ 4 -999 4 4 4 33 2 4 4 8 ]{61: LDI} # Load value of first r
[ 4 -999 4 4 4 33 2 4 4 4 ]{62: GETBP} # get bp
[ 4 -999 4 4 4 33 2 4 4 4 7 ]{63: CSTI 0} # push 0 to stack
[ 4 -999 4 4 4 33 2 4 4 4 7 0 ]{65: ADD} # add &bp + 0 (address of n)
[ 4 -999 4 4 4 33 2 4 4 4 7 ]{66: LDI} # load value of n
[ 4 -999 4 4 4 33 2 4 4 4 4 ]{67: GETBP} # get bp
[ 4 -999 4 4 4 33 2 4 4 4 4 7 ]{68: CSTI 0} # push 0 to stack
[ 4 -999 4 4 4 33 2 4 4 4 4 7 0 ]{70: ADD} # add &bp + 0 (address of n)
[ 4 -999 4 4 4 33 2 4 4 4 4 7 ]{71: LDI} # Load value of n
[ 4 -999 4 4 4 33 2 4 4 4 4 4 ]{72: MUL} # n * n
[ 4 -999 4 4 4 33 2 4 4 4 16 ]{73: STI} # stores value of n * n on adress of second r
[ 4 -999 4 4 16 33 2 4 4 16 ]{74: INCSP -1} # shrink stack
[ 4 -999 4 4 16 33 2 4 4 ]{76: INCSP 0} # not optimized code
[ 4 -999 4 4 16 33 2 4 4 ]{78: RET 1} # Discards stack frame, pushes the return value, restores the base pointer, and jumps to the return address.
[ 4 -999 4 4 16 4 ]{33: INCSP -1} # shrink stack, its a void we don't need the return value
[ 4 -999 4 4 16 ]{35: GETBP} # get bp 
[ 4 -999 4 4 16 2 ]{36: CSTI 2} # push 2 to stack
[ 4 -999 4 4 16 2 2 ]{38: ADD} # add &bp+2 (address of second r)
[ 4 -999 4 4 16 4 ]{39: LDI} # load value of second r
[ 4 -999 4 4 16 16 ]{40: PRINTI} # print value of second r
16 [ 4 -999 4 4 16 16 ]{41: INCSP -1} # shrink stack (discard loaded value of second r, that was used for print)
[ 4 -999 4 4 16 ]{43: INCSP -1} # shrink stack (removes second r entirely from stack)
[ 4 -999 4 4 ]{45: GETBP} # get bp
[ 4 -999 4 4 2 ]{46: CSTI 1} # push 1 to stack
[ 4 -999 4 4 2 1 ]{48: ADD} # add &bp + 1 (address of first r)
[ 4 -999 4 4 3 ]{49: LDI} # load value of first r
[ 4 -999 4 4 4 ]{50: PRINTI} # print value of first r
4 [ 4 -999 4 4 4 ]{51: INCSP -1} # shrink stack, to remove loaded value used for print
[ 4 -999 4 4 ]{53: INCSP -1} # shrink stack to remove first r completely
[ 4 -999 4 ]{55: RET 0} # return 0, discards stack frame
[ 4 ]{4: STOP} # HALT THE MACHINE!!!!!!!
```


Exercise 8.3:
Skal laves i Comp.fs i funktionen: cExpr

Exercise 8.4:

Exercise 8.5: 

Exercise 8.6:
