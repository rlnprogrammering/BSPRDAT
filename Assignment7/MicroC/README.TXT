--------------------------------------------------------------
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

** ex5 **
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


Stacktrace:

[ ]{0: LDARGS}
[ 1 ]{1: CALL 1 5}
[ 4 -999 1 ]{5: INCSP 1}
[ 4 -999 1 0 ]{7: GETBP}
[ 4 -999 1 0 2 ]{8: CSTI 1}
[ 4 -999 1 0 2 1 ]{10: ADD}
[ 4 -999 1 0 3 ]{11: GETBP}
[ 4 -999 1 0 3 2 ]{12: CSTI 0}
[ 4 -999 1 0 3 2 0 ]{14: ADD}
[ 4 -999 1 0 3 2 ]{15: LDI}
[ 4 -999 1 0 3 1 ]{16: STI}
[ 4 -999 1 1 1 ]{17: INCSP -1}
[ 4 -999 1 1 ]{19: INCSP 1}
[ 4 -999 1 1 1 ]{21: GETBP}
[ 4 -999 1 1 1 2 ]{22: CSTI 0}
[ 4 -999 1 1 1 2 0 ]{24: ADD}
[ 4 -999 1 1 1 2 ]{25: LDI}
[ 4 -999 1 1 1 1 ]{26: GETBP}
[ 4 -999 1 1 1 1 2 ]{27: CSTI 2}
[ 4 -999 1 1 1 1 2 2 ]{29: ADD}
[ 4 -999 1 1 1 1 4 ]{30: CALL 2 57}
[ 4 -999 1 1 1 33 2 1 4 ]{57: GETBP}
[ 4 -999 1 1 1 33 2 1 4 7 ]{58: CSTI 1}
[ 4 -999 1 1 1 33 2 1 4 7 1 ]{60: ADD}
[ 4 -999 1 1 1 33 2 1 4 8 ]{61: LDI}
[ 4 -999 1 1 1 33 2 1 4 4 ]{62: GETBP}
[ 4 -999 1 1 1 33 2 1 4 4 7 ]{63: CSTI 0}
[ 4 -999 1 1 1 33 2 1 4 4 7 0 ]{65: ADD}
[ 4 -999 1 1 1 33 2 1 4 4 7 ]{66: LDI}
[ 4 -999 1 1 1 33 2 1 4 4 1 ]{67: GETBP}
[ 4 -999 1 1 1 33 2 1 4 4 1 7 ]{68: CSTI 0}
[ 4 -999 1 1 1 33 2 1 4 4 1 7 0 ]{70: ADD}
[ 4 -999 1 1 1 33 2 1 4 4 1 7 ]{71: LDI}
[ 4 -999 1 1 1 33 2 1 4 4 1 1 ]{72: MUL}
[ 4 -999 1 1 1 33 2 1 4 4 1 ]{73: STI}
[ 4 -999 1 1 1 33 2 1 4 1 ]{74: INCSP -1}
[ 4 -999 1 1 1 33 2 1 4 ]{76: INCSP 0}
[ 4 -999 1 1 1 33 2 1 4 ]{78: RET 1}
[ 4 -999 1 1 1 4 ]{33: INCSP -1}
[ 4 -999 1 1 1 ]{35: GETBP}
[ 4 -999 1 1 1 2 ]{36: CSTI 2}
[ 4 -999 1 1 1 2 2 ]{38: ADD}
[ 4 -999 1 1 1 4 ]{39: LDI}
[ 4 -999 1 1 1 1 ]{40: PRINTI}
1 [ 4 -999 1 1 1 1 ]{41: INCSP -1}
[ 4 -999 1 1 1 ]{43: INCSP -1}
[ 4 -999 1 1 ]{45: GETBP}
[ 4 -999 1 1 2 ]{46: CSTI 1}
[ 4 -999 1 1 2 1 ]{48: ADD}
[ 4 -999 1 1 3 ]{49: LDI}
[ 4 -999 1 1 1 ]{50: PRINTI}
1 [ 4 -999 1 1 1 ]{51: INCSP -1}
[ 4 -999 1 1 ]{53: INCSP -1}
[ 4 -999 1 ]{55: RET 0}
[ 1 ]{4: STOP}



Exercise 8.3:
Skal laves i Comp.fs i funktionen: cExpr

Exercise 8.4:

Exercise 8.5: 

Exercise 8.6:


--------------------------------------------------------------


Compiling and loading the micro-C evaluator and parser (MicroC/README.TXT)
--------------------------------------------------------------------------

Archive microc.zip contains the files used in points A, B and C below.


A. Generating and compiling the lexer and parser for micro-C, and
   loading the interpreter for micro-C.

   fslex --unicode CLex.fsl
   fsyacc --module CPar CPar.fsy
   dotnet fsi -r ~/fsharp/FsLexYacc.Runtime.dll Util.fs Absyn.fs CPar.fs CLex.fs Parse.fs Interp.fs ParseAndRun.fs

   open ParseAndRun;;
   fromFile "ex1.c";;
   run (fromFile "ex1.c") [17];;
   run (fromFile "ex5.c") [4];;
   run (fromFile "ex11.c") [8];;


B. To compile and use the micro-C compiler

   fslex --unicode CLex.fsl
   fsyacc --module CPar CPar.fsy
   dotnet fsi -r ~/fsharp/FsLexYacc.Runtime.dll Util.fs Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs   

   open ParseAndComp;;
   compileToFile (fromFile "ex11.c") "ex11.out";;
   compile "ex11";;

The Java stack machine:

   javac Machine.java
   java Machine ex11.out 8

The C stack machine
   gcc -o machine machine.c
   ./machine ex11.out 8    

C. To compile and use the backwards (continuation-based) micro-C compiler:

   fslex --unicode CLex.fsl
   fsyacc --module CPar CPar.fsy
   dotnet fsi -r ~/fsharp/FsLexYacc.Runtime.dll Util.fs Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Contcomp.fs ParseAndContcomp.fs   

   open ParseAndContcomp;;
   contCompileToFile (fromFile "ex11.c") "ex11.out";;
   compile "ex11";;
   #q;;

   javac Machine.java
   java Machine ex11.out 8	


D. Build the backwards micro-C compiler as a command-line program microcc

   fslex --unicode CLex.fsl
   fsyacc --module CPar CPar.fsy

   dotnet build microcc.fsproj
   dotnet run ex11.c

   javac Machine.java
   java Machine ex11.out 8
