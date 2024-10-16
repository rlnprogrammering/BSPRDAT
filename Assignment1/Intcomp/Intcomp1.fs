(* Programming language concepts for software developers, 2012-02-17 *)

(* Evaluation, checking, and compilation of object language expressions *)
(* Stack machines for expression evaluation                             *) 

(* Object language expressions with variable bindings and nested scope *)

module Intcomp1

type expr = 
  | CstI of int
  | Var of string
  | Let of (string * expr) list * expr
  | Prim of string * expr * expr;;

(* Example of what has to work *)

// Let ([("x1", ...); ("x2", ...)], Prim("+", Var "x1", Var "x2"))

let a1 = Let ([("x1", CstI 17); ("x2", CstI 22)], Prim("+", Var "x1", Prim("*", CstI 100, Var "x2")))
(* Some closed expressions: *)

// let z = 17 in z + z = 34
let e1 = Let("z", CstI 17, Prim("+", Var "z", Var "z"));;

// (let z = 17 in (let z = 22 in 100 * z) + z) = 2217
// 100 * 22 + 17
let e2 = Let("z", CstI 17, 
             Prim("+", Let("z", CstI 22, Prim("*", CstI 100, Var "z")),
                       Var "z"));;

// let z = 5 - 4 in 100 * z = 100
// 100 * 1
let e3 = Let("z", Prim("-", CstI 5, CstI 4), 
             Prim("*", CstI 100, Var "z"));;

// (20 + (let z = 17 in z +2) + 30) = 69
// 20 + (17 + 2) + 30
let e4 = Prim("+", Prim("+", CstI 20, Let("z", CstI 17, 
                                          Prim("+", Var "z", CstI 2))),
                   CstI 30);;

// (2 * (let x = 3 in x + 4)) = 14
// (2 * (3 + 4)) = 14
let e5 = Prim("*", CstI 2, Let("x", CstI 3, Prim("+", Var "x", CstI 4)));;

(* ---------------------------------------------------------------------- *)

(* Evaluation of expressions with variables and bindings *)

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | Let((bindings : (string * expr) list), (ebody : expr)) ->
        let env1 = 
            bindings |> List.fold (fun acc (name, expr) -> (name, eval expr acc) :: acc) env
        eval ebody env1
    | Prim("+", e1, e2) -> eval e1 env + eval e2 env
    | Prim("*", e1, e2) -> eval e1 env * eval e2 env
    | Prim("-", e1, e2) -> eval e1 env - eval e2 env
    | Prim _            -> failwith "unknown primitive";;

let a1 = Let ([("x1", CstI 17); ("x2", CstI 22)], 
              Prim("+", Var "x1", 
              Prim("*", CstI 100, Var "x2")))

let run e = eval e [];;

let r1 = run e1;;
let r2 = run e2;;
let r3 = run e3;;
let r4 = run e4;;
let r5 = run e5;;

// exercise 2.1
let r1 = run a1;;

(* 
- let r1 = run e1;;
val r1: int = 34

> let r2 = run e2;;
val r2: int = 2217

> let r3 = run e3;;
val r3: int = 100

> let r4 = run e4;;
val r4: int = 69

> let r5 = run e5;;
val r5: int = 14
*)

(* ---------------------------------------------------------------------- *)

(* Checking whether an expression is closed.  The vs is 
   a list of the bound variables.  *)
(* ---------------------------------------------------------------------- *)

(* Closedness *)

// let mem x vs = List.exists (fun y -> x=y) vs;;

let rec mem x vs = 
    match vs with
    | []      -> false
    | v :: vr -> x=v || mem x vr;;

(* Checking whether an expression is closed.  The vs is 
   a list of the bound variables.  *)

let rec closedin (e : expr) (vs : string list) : bool =
    match e with
    | CstI i -> true
    | Var x  -> List.exists (fun y -> x=y) vs
    | Let(x, erhs, ebody) -> 
      let vs1 = x :: vs 
      closedin erhs vs && closedin ebody vs1
    | Prim(ope, e1, e2) -> closedin e1 vs && closedin e2 vs;;

(* An expression is closed if it is closed in the empty environment *)

let closed1 e = closedin e [];;


(* ---------------------------------------------------------------------- *)

(* Substitution of expressions for variables *)

(* This version of lookup returns a Var(x) expression if there is no
   pair (x,e) in the list env --- instead of failing with exception: *)

let rec lookOrSelf env x =
    match env with 
    | []        -> Var x
    | (y, e)::r -> if x=y then e else lookOrSelf r x;;

(* Remove (x, _) from env: *)

let rec remove env x =
    match env with 
    | []        -> []
    | (y, e)::r -> if x=y then r else (y, e) :: remove r x;;

(* Naive substitution, may capture free variables: *)

let rec nsubst (e : expr) (env : (string * expr) list) : expr =
    match e with
    | CstI i -> e
    | Var x  -> lookOrSelf env x
    | Let(x, erhs, ebody) ->
      let newenv = remove env x
      Let(x, nsubst erhs env, nsubst ebody newenv)
    | Prim(ope, e1, e2) -> Prim(ope, nsubst e1 env, nsubst e2 env)

(* Some expressions with free variables: *)

let e6 = Prim("+", Var "y", Var "z");;

let e6s1 = nsubst e6 [("z", CstI 17)];;

let e6s2 = nsubst e6 [("z", Prim("-", CstI 5, CstI 4))];;

let e6s3 = nsubst e6 [("z", Prim("+", Var "z", Var "z"))];;

// Shows that only z outside the Let gets substituted:
let e7 = Prim("+", Let("z", CstI 22, Prim("*", CstI 5, Var "z")),
                   Var "z");;

let e7s1 = nsubst e7 [("z", CstI 100)];;

// Shows that only the z in the Let rhs gets substituted
let e8 = Let("z", Prim("*", CstI 22, Var "z"), Prim("*", CstI 5, Var "z"));;

let e8s1 = nsubst e8 [("z", CstI 100)];;

// Shows (wrong) capture of free variable z under the let:
let e9 = Let("z", CstI 22, Prim("*", Var "y", Var "z"));;

let e9s1 = nsubst e9 [("y", Var "z")];;

// 
let e9s2 = nsubst e9 [("z", Prim("-", CstI 5, CstI 4))];;

let newVar : string -> string = 
    let n = ref 0
    let varMaker x = (n := 1 + !n; x + string (!n))
    varMaker

(* Correct, capture-avoiding substitution *)

let rec subst (e : expr) (env : (string * expr) list) : expr =
    match e with
    | CstI i -> e
    | Var x  -> lookOrSelf env x
    | Let(x, erhs, ebody) ->
      let newx = newVar x
      let newenv = (x, Var newx) :: remove env x
      Let(newx, subst erhs env, subst ebody newenv)
    | Prim(ope, e1, e2) -> Prim(ope, subst e1 env, subst e2 env)

let e6s1a = subst e6 [("z", CstI 17)];;

let e6s2a = subst e6 [("z", Prim("-", CstI 5, CstI 4))];;

let e6s3a = subst e6 [("z", Prim("+", Var "z", Var "z"))];;


// Shows renaming of bound variable z (to z1)
let e7s1a = subst e7 [("z", CstI 100)];;

// Shows renaming of bound variable z (to z2)
let e8s1a = subst e8 [("z", CstI 100)];;

// Shows renaming of bound variable z (to z3), avoiding capture of free z
let e9s1a = subst e9 [("y", Var "z")];;

(* ---------------------------------------------------------------------- *)

(* Free variables *)

(* Operations on sets, represented as lists.  Simple but inefficient;
   one could use binary trees, hashtables or splaytrees for
   efficiency.  *)

(* union(xs, ys) is the set of all elements in xs or ys, without duplicates *)

let rec union (xs, ys) = 
    match xs with 
    | []    -> ys
    | x::xr -> if mem x ys then union(xr, ys)
               else x :: union(xr, ys);;

(* minus xs ys  is the set of all elements in xs but not in ys *)

let rec minus (xs, ys) = 
    match xs with 
    | []    -> []
    | x::xr -> if mem x ys then minus(xr, ys)
               else x :: minus (xr, ys);;

(* Find all variables that occur free in expression e *)

// let x1 = x1 + 7; // x1 is free
// let x1 + 8; // x1 is bound

// let x1 = x1 + 7 in x1 + 8 end // x1 is bound

let rec freevars e : string list =
    match e with
    | CstI i -> []
    | Var x  -> [x]
    | Let(bindings, ebody) ->
          bindings |> List.rev |> List.fold (fun acc (name, expr) -> 
          union (freevars expr, minus (acc, [name]))) []
    | Prim(ope, e1, e2) -> union (freevars e1, freevars e2);;

(* Alternative definition of closed *)

let closed2 e = (freevars e = []);;

let a2 = Let ([("x1", CstI 17); ("x2", Var "y")], Prim("+", Var "x1", Prim("*", CstI 100, Var "x2")))

let a3 = Let(
    [
        ("x1", Prim("+", CstI 5, CstI 7)); 
        ("x2", Prim("*", Var "x1", CstI 2))
    ], 
    Prim("+", Var "x1", Var "x2"))

let a4 = Let(
    [
        ("x1", Prim("+", Var "x1", CstI 7))
    ],
    Prim("+", Var "x1", CstI 8)
)

let free1 = freevars a2;;
let free2 = freevars a3;;
let free3 = freevars a4;;

(* ---------------------------------------------------------------------- *)

(* Compilation to target expressions with numerical indexes instead of
   symbolic variable names.  *)

type texpr =                            (* target expressions *)
  | TCstI of int
  | TVar of int                         (* index into runtime environment *)
  | TLet of texpr * texpr               (* erhs and ebody                 *)
  | TPrim of string * texpr * texpr;;


(* Map variable name to variable index at compile-time *)

let rec getindex vs x = 
    match vs with 
    | []    -> failwith "Variable not found"
    | y::yr -> if x=y then 0 else 1 + getindex yr x;;

(* Compiling from expr to texpr *)

let rec tcomp (e : expr) (cenv : string list) : texpr =
    match e with
    | CstI i -> TCstI i
    | Var x  -> TVar (getindex cenv x)
    | Let(bindings, ebody) -> 
        let cenv1 = bindings |> List.fold (fun acc (name, expr) -> name::acc) cenv
        let head = snd (List.head bindings)
        TLet(tcomp head cenv, tcomp ebody cenv1)
    | Prim(ope, e1, e2) -> TPrim(ope, tcomp e1 cenv, tcomp e2 cenv);;

(* Evaluation of target expressions with variable indexes.  The
   run-time environment renv is a list of variable values (ints).  *)

let rec teval (e : texpr) (renv : int list) : int =
    match e with
    | TCstI i -> i
    | TVar n  -> List.nth renv n
    | TLet(erhs, ebody) -> 
      let xval = teval erhs renv
      let renv1 = xval :: renv 
      teval ebody renv1 
    | TPrim("+", e1, e2) -> teval e1 renv + teval e2 renv
    | TPrim("*", e1, e2) -> teval e1 renv * teval e2 renv
    | TPrim("-", e1, e2) -> teval e1 renv - teval e2 renv
    | TPrim _            -> failwith "unknown primitive";;

(* Correctness: eval e []  equals  teval (tcomp e []) [] *)

let t1 = tcomp a1 [];;
let t2 = tcomp a2 [];;
let t3 = tcomp a3 [];;

(* ---------------------------------------------------------------------- *)

(* Compilation to a simple stack machine *)


(* ---------------------------------------------------------------------- *)

(* Stack machines *)

(* Stack machine instructions.  An expressions in postfix or reverse
   Polish form is a list of stack machine instructions. *)

type rinstr =
  | RCstI of int
  | RAdd 
  | RSub
  | RMul 
  | RDup
  | RSwap;;

(* A simple stack machine for evaluation of variable-free expressions
   in postfix form *)

let rec reval (inss : rinstr list) (stack : int list) : int =
    match (inss, stack) with 
    | ([], v :: _) -> v
    | ([], [])     -> failwith "reval: no result on stack!"
    | (RCstI i :: insr,             stk)  -> reval insr (i::stk)
    | (RAdd    :: insr, i2 :: i1 :: stkr) -> reval insr ((i1+i2)::stkr)
    | (RSub    :: insr, i2 :: i1 :: stkr) -> reval insr ((i1-i2)::stkr)
    | (RMul    :: insr, i2 :: i1 :: stkr) -> reval insr ((i1*i2)::stkr)
    | (RDup    :: insr,       i1 :: stkr) -> reval insr (i1 :: i1 :: stkr)
    | (RSwap   :: insr, i2 :: i1 :: stkr) -> reval insr (i1 :: i2 :: stkr)
    | _ -> failwith "reval: too few operands on stack";;

let rpn1 = reval [RCstI 10; RCstI 17; RDup; RMul; RAdd] [];;


(* Compilation of a variable-free expression to a rinstr list *)

let rec rcomp (e : expr) : rinstr list =
    match e with
    | CstI i            -> [RCstI i]
    | Var _             -> failwith "rcomp cannot compile Var"
    | Let _             -> failwith "rcomp cannot compile Let"
    | Prim("+", e1, e2) -> rcomp e1 @ rcomp e2 @ [RAdd]
    | Prim("*", e1, e2) -> rcomp e1 @ rcomp e2 @ [RMul]
    | Prim("-", e1, e2) -> rcomp e1 @ rcomp e2 @ [RSub]
    | Prim _            -> failwith "unknown primitive";;
            
(* Correctness: eval e []  equals  reval (rcomp e) [] *)


(* Storing intermediate results and variable bindings in the same stack *)

type sinstr =
  | SCstI of int                        (* push integer           *)
  | SVar of int                         (* push variable from env *)
  | SAdd                                (* pop args, push sum     *)
  | SSub                                (* pop args, push diff.   *)
  | SMul                                (* pop args, push product *)
  | SPop                                (* pop value/unbind var   *)
  | SSwap;;                             (* exchange top and next  *)
 
let rec seval (inss : sinstr list) (stack : int list) =
    match (inss, stack) with
    | ([], v :: _) -> v
    | ([], [])     -> failwith "seval: no result on stack"
    | (SCstI i :: insr,          stk) -> seval insr (i :: stk) 
    | (SVar i  :: insr,          stk) -> seval insr (List.nth stk i :: stk) 
    | (SAdd    :: insr, i2::i1::stkr) -> seval insr (i1+i2 :: stkr)
    | (SSub    :: insr, i2::i1::stkr) -> seval insr (i1-i2 :: stkr)
    | (SMul    :: insr, i2::i1::stkr) -> seval insr (i1*i2 :: stkr)
    | (SPop    :: insr,    _ :: stkr) -> seval insr stkr
    | (SSwap   :: insr, i2::i1::stkr) -> seval insr (i1::i2::stkr)
    | _ -> failwith "seval: too few operands on stack";;


(* A compile-time variable environment representing the state of
   the run-time stack. *)

type stackvalue =
  | Value                               (* A computed value *)
  | Bound of string;;                   (* A bound variable *)

(* Compilation to a list of instructions for a unified-stack machine *)

let rec scomp (e : expr) (cenv : stackvalue list) : sinstr list =
    match e with
    | CstI i -> [SCstI i]
    | Var x  -> [SVar (getindex cenv (Bound x))]
    | Let(x, erhs, ebody) -> 
          scomp erhs cenv @ scomp ebody (Bound x :: cenv) @ [SSwap; SPop]
    | Prim("+", e1, e2) -> 
          scomp e1 cenv @ scomp e2 (Value :: cenv) @ [SAdd] 
    | Prim("-", e1, e2) -> 
          scomp e1 cenv @ scomp e2 (Value :: cenv) @ [SSub] 
    | Prim("*", e1, e2) -> 
          scomp e1 cenv @ scomp e2 (Value :: cenv) @ [SMul] 
    | Prim _ -> failwith "scomp: unknown operator";;

let s1 = scomp e1 [];;
let s2 = scomp e2 [];;
let s3 = scomp e3 [];;
let s5 = scomp e5 [];;

(* Output the integers in list inss to the text file called fname: *)

let intsToFile (inss : int list) (fname : string) = 
    let text = String.concat " " (List.map string inss)
    System.IO.File.WriteAllText(fname, text);;

(* -----------------------------------------------------------------  *)
