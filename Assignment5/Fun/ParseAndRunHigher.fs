(* File Fun/ParseAndRunHigher.fs *)

module ParseAndRunHigher

open HigherFun;;

let fromString = Parse.fromString;;

let eval = HigherFun.eval;;

let run e = eval e [];;

(* Examples of higher-order programs, in concrete syntax *)

let ex5 = 
    fromString 
     @"let tw g = let app x = g (g x) in app end 
       in let mul3 x = 3 * x 
       in let quad = tw mul3 
       in quad 7 end end end";;

let ex6 = 
    fromString 
     @"let tw g = let app x = g (g x) in app end 
       in let mul3 x = 3 * x 
       in let quad = tw mul3 
       in quad end end end";;

let ex7 = 
    fromString 
     @"let rep n = 
           let rep1 g = 
               let rep2 x = if n=0 then x else rep (n-1) g (g x) 
               in rep2 end 
           in rep1 end 
       in let mul3 x = 3 * x 
       in let tw = rep 2 
       in let quad = tw mul3 
       in quad 7 end end end end";;

let ex8 = 
    fromString 
     @"let rep n =
           let rep1 g = 
               let rep2 x = if n=0 then x else rep (n-1) g (g x) 
               in rep2 end 
           in rep1 end 
       in let mul3 x = 3 * x 
       in let twototen = rep 10 mul3 
       in twototen 7 end end end";;

let ex9 = 
    fromString 
     @"let rep n = 
           let rep1 g = 
               let rep2 x = if n=0 then x else rep (n-1) g (g x) 
               in rep2 end 
           in rep1 end 
       in let mul3 x = 3 * x 
       in let twototen = (rep 10) mul3 
       in twototen 7 end end end";;



(* Exercise 6.1 *)
let ex611 = 
        fromString 
            @"let add x = let f y = x+y in f end
              in add 2 5 end";;

let ex612 = 
        fromString
            @"let add x = let f y = x+y in f end
              in let addtwo = add 2
              in addtwo 5 end
              end";;

let ex613 = 
        fromString
            @"let add x = let f y = x+y in f end
              in let addtwo = add 2
              in let x = 77 in addtwo 5 end
              end
              end";;

let ex614 = 
        fromString
            @"let add x = let f y = x+y in f end
              in add 2 end";; 
(*
run ex611;;
run ex612;;
run ex613;;
run ex614;; 
*)
(* explaination of ex614: 'add 2' is a higher order function, that returns f with x bound to 2
    The returned function f is a closure because it captures the environment where x is 2. *)



(* Exercise 6.2 *)
let ex621 = fromString @"fun x -> 2*x";;
// run ex621;;

let ex622 = fromString @"let y = 22 in fun z -> z+y end";;
// run ex622;;



(* Exercise 6.3 *)
let ex631 = fromString @"let add x = fun y -> x+y
                            in add 2 5 end";;
// run ex631;;

let ex632 = fromString @"let add = fun x -> fun y -> x+y
                            in add 2 5 end";;
// run ex632;;



(* Exercise 6.4 *)
let ex641 = fromString @"let f x = 1 in f f end";;
// run ex641;;

let ex642 = fromString @"let f x = if x<10 then 42 else f(x+1)
                            in f 20 end";;
// run ex642;; // (infinite loop!)


(* Exercise 6.5 (1) *)
open ParseAndType;;
let inferType = ParseAndType.inferType;;

(* int *)
let ex6511 = inferType (fromString "let f x = 1 in f f end");;

(* error "circularity" (g calls itself recursively without termination) *)
// let ex6512 = inferType (fromString "let f g = g g in f end");; 

(* bool *)
let ex6513 = inferType (fromString @"let f x =
                                        let g y = y
                                        in g false end
                                    in f 42 end");;

(* error "bool and int" (should only have one return type) *)
// let ex6514 = inferType (fromString @"let f x =
//                                         let g y = if true then y else x
//                                         in g false end
//                                     in f 42 end");;

(* bool *)
let ex6515 = inferType (fromString @"let f x =
                                        let g y = if true then y else x
                                        in g false end
                                    in f true end");;


(* Exercise 6.5 (2) *)
// bool -> bool
let ex6521 = inferType (fromString "let f x = if x then true else false in f end");; 

// int -> int
let ex6522 = inferType (fromString "let f x = x+x in f end");;

// int -> int -> int
let ex6523 = inferType (fromString @"let f x = let g y = x+y in g end in f end");;

// ’a -> ’b -> ’a
let ex6524 = inferType (fromString @"let f x = let g y = x in g end in f end");;

// ’a -> ’b -> ’b
let ex6525 = inferType (fromString @"let f x = let g y = y in g end in f end");;

// (’a -> ’b) -> (’b -> ’c) -> (’a -> ’c)
let ex6526 = inferType (fromString @"let f x = let g y = let h z = y (x z) in h end in g end in f end");;

// ’a -> ’b
let ex6527 = inferType (fromString @"let f x = f x in f end");;

// ’a
let ex6528 = inferType (fromString @"let f x = f x in f 42 end");;
