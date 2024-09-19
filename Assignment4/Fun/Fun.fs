(* File Fun/Fun.fs
   A strict functional language with integers and first-order 
   one-argument functions * sestoft@itu.dk

   Does not support mutually recursive function bindings.

   Performs tail recursion in constant space (because F# does).
*)

module Fun

open Absyn

(* Environment operations *)

type 'v env = (string * 'v) list

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

(* A runtime value is an integer or a function closure *)

type value = 
  | Int of int
  | Closure of string * string * expr * value env       (* (f, x, fBody, fDeclEnv) *)

let rec eval (e : expr) (env : value env) : int =
    match e with 
    | CstI i -> i
    | CstB b -> if b then 1 else 0
    | Var x  ->
      match lookup env x with
      | Int i -> i 
      | _     -> failwith "eval Var"
    | Prim(ope, e1, e2) -> 
      let i1 = eval e1 env
      let i2 = eval e2 env
      match ope with
      | "*" -> i1 * i2
      | "+" -> i1 + i2
      | "-" -> i1 - i2
      | "=" -> if i1 = i2 then 1 else 0
      | "<" -> if i1 < i2 then 1 else 0
      | _   -> failwith ("unknown primitive " + ope)
    | Let(x, eRhs, letBody) -> 
      let xVal = Int(eval eRhs env)
      let bodyEnv = (x, xVal) :: env
      eval letBody bodyEnv
    | If(e1, e2, e3) -> 
      let b = eval e1 env
      if b<>0 then eval e2 env
      else eval e3 env
    | Letfun(f, x, fBody, letBody) -> 
      let bodyEnv = (f, Closure(f, x, fBody, env)) :: env 
      eval letBody bodyEnv
    | Call(Var f, eArg) -> 
      let fClosure = lookup env f
      match fClosure with
      | Closure (f, x, fBody, fDeclEnv) ->
        let xVal = Int(eval eArg env)
        let fBodyEnv = (x, xVal) :: (f, fClosure) :: fDeclEnv
        eval fBody fBodyEnv
      | _ -> failwith "eval Call: not a function"
    | Call _ -> failwith "eval Call: not first-order function"

(* Evaluate in empty environment: program must have no free variables: *)

let run e = eval e [];;

(* Examples in abstract syntax *)

let ex1 = Letfun("f1", "x", Prim("+", Var "x", CstI 1), 
                 Call(Var "f1", CstI 12));;

(* Example: factorial *)

let ex2 = Letfun("fac", "x",
                 If(Prim("=", Var "x", CstI 0),
                    CstI 1,
                    Prim("*", Var "x", 
                              Call(Var "fac", 
                                   Prim("-", Var "x", CstI 1)))),
                 Call(Var "fac", Var "n"));;

(* let fac10 = eval ex2 [("n", Int 10)];; *)

(* Example: deep recursion to check for constant-space tail recursion *)

let ex3 = Letfun("deep", "x", 
                 If(Prim("=", Var "x", CstI 0),
                    CstI 1,
                    Call(Var "deep", Prim("-", Var "x", CstI 1))),
                 Call(Var "deep", Var "count"));;
    
let rundeep n = eval ex3 [("count", Int n)];;

(* Example: static scope (result 14) or dynamic scope (result 25) *)

let ex4 =
    Let("y", CstI 11,
        Letfun("f", "x", Prim("+", Var "x", Var "y"),
               Let("y", CstI 22, Call(Var "f", CstI 3))));;

(* Example: two function definitions: a comparison and Fibonacci *)

let ex5 = 
    Letfun("ge2", "x", Prim("<", CstI 1, Var "x"),
           Letfun("fib", "n",
                  If(Call(Var "ge2", Var "n"),
                     Prim("+",
                          Call(Var "fib", Prim("-", Var "n", CstI 1)),
                          Call(Var "fib", Prim("-", Var "n", CstI 2))),
                     CstI 1), Call(Var "fib", CstI 25)));;

(* Exercise 4.2 *)        
// let rec sum n =
//   match n with
//   | 1 -> 1
//   | _ -> n + (sum (n-1));;

let ex6 = 
    Letfun("sum", "n",
          If(Prim("=", Var "n", CstI 1),
                CstI 1,
                Prim("+", Var "n", Call(Var "sum", Prim("-", Var "n", CstI 1)))),
          Call(Var "sum", Var "count"));;

let sum n = eval ex6 [("count", Int n)];;

// let rec pow n e =
//   match e with
//   | 0 -> 1
//   | 1 -> n
//   | _ -> n * (pow n (e-1));;

let ex7 = 
  Let("n", CstI 3,
    Letfun("pow", "e",
          If(Prim("=", Var "e", CstI 0),
                CstI 1,
                If(Prim("=", Var "e", CstI 1),
                      Var "n",
                      Prim("*", Var "n", Call(Var "pow", Prim("-", Var "e", CstI 1))))),
          Call(Var "pow", CstI 8)));;


// let rec pow n e =
//   match e with
//   | 0 -> 1
//   | 1 -> n
//   | _ -> n * (pow n (e-1));;

// let rec powsum n e =
//   match e with
//   | 0 -> 1
//   | _ -> (pow n e) + (powsum n (e-1));;

