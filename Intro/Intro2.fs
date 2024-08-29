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


