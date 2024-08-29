(* Programming language concepts for software developers, 2010-08-28 *)

(* Evaluating simple expressions with variables *)

module Intro2

(* Association lists map object language variables to their values *)

let env = [("a", 3); ("c", 78); ("baf", 666); ("b", 111)];;

let emptyenv = []; (* the empty environment *)

let rec lookup env x =
    match env with 
    | []        -> failwith (x + " not found")
    | (y, v)::r -> if x=y then v else lookup r x;;

let cvalue = lookup env "c";;


(* Object language expressions with variables *)

type expr = 
  | CstI of int
  | Var of string
  | Prim of string * expr * expr
  | If of expr * expr * expr;;

let e1 = CstI 17;;

let e2 = Prim("+", CstI 3, Var "a");;

let e3 = Prim("+", Prim("*", Var "b", CstI 9), Var "a");;

let e4 = Prim("==", CstI 4, CstI 4);;

let e5 = Prim("max", CstI 5, CstI 4);;

let e6 = Prim("min", CstI 5, CstI 4);;


(* Evaluation within an environment *)

(* Excercise 1.1: (i), (ii) *)
let rec evalOld e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | Prim("+", e1, e2) -> eval e1 env + eval e2 env
    | Prim("*", e1, e2) -> eval e1 env * eval e2 env
    | Prim("-", e1, e2) -> eval e1 env - eval e2 env
    | Prim("==", e1, e2) -> 
      if eval e1 env = eval e2 env
      then 1
      else 0
    | Prim("max", e1, e2) -> 
      if eval e1 env > eval e2 env
      then eval e1 env
      else eval e2 env
    | Prim("min", e1, e2) ->
      if eval e1 env < eval e2 env
      then eval e1 env
      else eval e2 env
    | Prim _            -> failwith "unknown primitive";;

let e1v  = evalOld e1 env;;
let e2v1 = evalOld e2 env;;
let e2v2 = evalOld e2 [("a", 314)];;
let e3v  = evalOld e3 env;;
let e4v  = evalOld e4 env;;
let e5v  = evalOld e5 env;;
let e6v  = evalOld e6 env;;

(* Excercise 1.1: (iii) *)

let rec eval e (env : (string * int) list) : int =
    match e with
    | CstI i            -> i
    | Var x             -> lookup env x 
    | Prim(ope, e1, e2) -> 
      let i1 = eval e1 env in
      let i2 = eval e2 env in
      match ope with
      | "+" -> i1 + i2
      | "-" -> i1 - i2
      | "*" -> i1 * i2
      | "==" -> if i1 = i2 then 1 else 0
      | "max" -> if i1 > i2 then i1 else i2
      | "min" -> if i1 < i2 then i1 else i2
    | If(e1, e2, e3) -> 
      if eval e1 env <> 0
      then eval e2 env
      else eval e3 env
    | Prim _            -> failwith "unknown primitive";;


let e1v  = eval e1 env;;
let e2v1 = eval e2 env;;
let e2v2 = eval e2 [("a", 314)];;
let e3v  = eval e3 env;;
let e4v  = eval e4 env;;
let e5v  = eval e5 env;;
let e6v  = eval e6 env;;

(* (IV) & (V) *)
let e7 = If(Var "a", CstI 11, CstI 22);;
let e7v = eval e7 env;;


(* Excercise 1.2: (i) *)
type aexpr = 
  | CstI of int
  | Var of string
  | Add of aexpr * aexpr
  | Mul of aexpr * aexpr
  | Sub of aexpr * aexpr;;


// (ii)
let ae1 = Sub(Var "v", Add(Var "w", Var "z"));;
let ae2 = Mul(CstI 2, Sub(Var "v", Add(Var "w", Var "z")));;
let ae21 = Mul(CstI 2, ae1);;
let ae3 = Add(Var "x", Add(Var "y", Add(Var "z", Var "v")));;
let ae31 = Add(Add(Var "x", Var "y"), Add(Var "z", Var "v"));;
let ae32 = Add(Add(Add(Var "x", Var "y"), Var "z"), Var "v");;

(* (iii) *)
let rec fmt aexpr : string = 
    match aexpr with
    | CstI i -> string i
    | Var x -> x
    | Add(e1, e2) -> "(" + fmt e1 + " + " + fmt e2 + ")"
    | Mul(e1, e2) -> "(" + fmt e1 + " * " + fmt e2 + ")"
    | Sub(e1, e2) -> "(" + fmt e1 + " - " + fmt e2 + ")";;


let ae1f = fmt ae1;;
let ae2f = fmt ae2;;
let ae21f = fmt ae21;;
let ae3f = fmt ae3;;
let ae31f = fmt ae31;;
let ae32f = fmt ae32;;

(* (iv) *)

let rec simplify aexpr : aexpr = 
  match aexpr with
  | CstI i -> CstI i
  | Var x -> Var x
  | Add(e1, e2) -> 
    let i1 = simplify e1 in
    let i2 = simplify e2 in
      match (i1, i2) with
      | (CstI 0, i2) -> simplify(i2)
      | (i1, CstI 0) -> simplify(i1)
      | (i1, i2) -> simplify(Add(i1, i2))
  | Sub(e1, e2) ->
    let i1 = simplify e1 in
    let i2 = simplify e2 in
      match (i1, i2) with
      | (CstI 0, i2) -> simplify(i2)
      | (i1, CstI 0) -> simplify(i1)
      | (i1, i2) -> 
        if i1 <> i2 then simplify(Sub(i1, i2))
        else CstI 0
  | Mul(e1, e2) ->
    let i1 = simplify e1 in
    let i2 = simplify e2 in
      match (i1, i2) with
      | (CstI 0, i2) -> CstI 0
      | (i1, CstI 0) -> CstI 0
      | (CstI 1, i2) -> simplify(i2)
      | (i1, CstI 1) -> simplify(i1)
      | (i1, i2) -> simplify(Mul(i1, i2));;

let aes = Add(Var "x", CstI 0);;
let aeSub = Sub(Var "x", Var "x");;
let aeMul = Mul(Var "x", CstI 1);;



let aes1 = simplify aes;;
let aes2 = simplify aeSub;;
  
(* V *)

