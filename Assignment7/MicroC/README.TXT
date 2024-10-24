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
[  LDARGS; CALL (1, "L1"); STOP; 
   Label "L1";                   - Main
   INCSP 1;                      - Dec(TypI, ”i”), allocate i
   GETBP; CSTI 1; ADD;           - AccVar ”i”, get address of i at offset 1
   CSTI 0;                       - CstI 0, put 0 on the stack
   STI;                          - Store 42 in address i
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

Exercise 8.3:
Skal laves i Comp.fs i funktionen: cExpr
void main(int n) { 
    n = n + 1;
}
Bliver compilet til:
[LDARGS; CALL (1, "L1"); STOP; Label "L1"; GETBP; CSTI 0; ADD; GETBP; CSTI 0;
   ADD; LDI; CSTI 1; ADD; STI; INCSP -1; INCSP 0; RET 0]

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
