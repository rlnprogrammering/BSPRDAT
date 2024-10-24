--------------------------------------------------------------
Hello TA :D


Exercise 8.1:
(i) Udført ved at følge instruks B i denne README.
(ii) Write up the bytecode in a more structured way with labels only at the beginning of the line (as in this chapter):
Kør:
   open ParseAndComp;;
   fromFile "ex3.c";;
Få: 
   Prog
    [Fundec
       (None, "main", [(TypI, "n")],
        Block
          [Dec (TypI, "i"); Stmt (Expr (Assign (AccVar "i", CstI 0)));
           Stmt
             (While
                (Prim2 ("<", Access (AccVar "i"), Access (AccVar "n")),
                 Block
                   [Stmt (Expr (Prim1 ("printi", Access (AccVar "i"))));
                    Stmt
                      (Expr
                         (Assign
                            (AccVar "i",
                             Prim2 ("+", Access (AccVar "i"), CstI 1))))]))])]

compileToFile (fromFile "ex3.c") "ex3.out";; 
og formatér og skriv kommentarer ud fra ovenstående:
[  LDARGS;                       - Loads n on the stack
   CALL (1, "L1"); STOP; 

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
Prøve gennemgang af stacken for ex3.c hvor n=1:
[]
[n] - LDARGS n
[r|bp|n] - Call (1, "L1")
[r|bp|n| ] - INCSP 1
[r|bp|n| |bp] - GETBP
[r|bp|n| |bp|1] - CSTI 1
[r|bp|n| |bp+1] - ADD
[r|bp|n| |bp+1|0] - CSTI 0
[r|bp|n|0|0] - STI
[r|bp|n|0] - INCSP -1
[r|bp|n|0] - goto (ingen ændring) (jump til condition check "L3")
*vi har nu n og i på stakken og bp peger på n*

*L3*
[r|bp|n|0|bp|1] - GETBP;CSTI 1;
[r|bp|n|0|bp+1] - ADD
[r|bp|n|0|0] - LDI (load addres af i)
[r|bp|n|0|0|bp|0] - GETBP;CSTI 0;
[r|bp|n|0|0|bp+0] - ADD
[r|bp|n|0|0|n] - LDI (load addres af n)
[r|bp|n|0|1] - LT (0 < n) hvilket er true så resultatet er 1
[r|bp|n|0] - IFNZRO (jump til block "L2")

*L2*
[r|bp|n|0|bp|1] - GETBP;CSTI 1;
[r|bp|n|0|bp+1] - ADD
[r|bp|n|0|bp+1|bp|1] - GETBP;CSTI 1;
[r|bp|n|0|bp+1|bp+1] - ADD
[r|bp|n|0|bp+1|0] - LDI
[r|bp|n|0|bp+1|0|1] - CSTI 1
[r|bp|n|0|bp+1|1] - ADD
[r|bp|n|1|1] - STI
[r|bp|n|1] - INCSP -1
[r|bp|n|1] - INCSP 0 (gør ingenting? koden er ikke optimeret)

*L3*
[r|bp|n|1|bp|1] - GETBP;CSTI 1;
[r|bp|n|1|bp+1] - ADD
[r|bp|n|1|1] - LDI (load addres af i)
[r|bp|n|1|1|bp|0] - GETBP;CSTI 0;
[r|bp|n|1|1|bp+0] - ADD
[r|bp|n|1|1|n] - LDI (load addres af n)
[r|bp|n|1|1] - LT (1 < n) hvilket er false da n = 1 så resultatet er 0
[r|bp|n|1] - IFNZRO (jumper ikke)
[r|bp|n] - INCSP -1
[] - RET 0



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
