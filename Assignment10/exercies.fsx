(* Execise 11.1 *)
let rec len xs =
    match xs with
    | [] -> 0
    | x::xr -> 1 + len xr;;

// (i) continuation
let rec lenc xs c =
    match xs with
    | [] -> c 0
    | x::xr -> lenc xr (fun r -> c (r + 1))

// (ii)
// > lenc [1;1;1;1] (fun v -> 2*v);;
// val it: int = 8

// (iii) tail recursive
let rec leni xs acc =
    match xs with
    | [] -> acc
    | x::xr -> leni xr (acc+1);;

// What is the relation between lenc and leni?
// 1. Both lenc and leni are tail-recursive functions for calculating the length of a list.
// 2. lenc a continuation function to pass intermediate results. Can optimize memory usage by avoiding deep recursion and enabling tail call optimization.
// 3. leni uses an accumulator to store intermediate results directly. Efficient in terms of stack usage, as it avoids creating additional stack frames.


(* Execise 11.2 *)
let rec rev xs =
    match xs with
    | [] -> []
    | x::xr -> rev xr @ [x];;

// (i)
let rec revc xs c =
    match xs with
    | [] -> c []
    | x::xr -> revc xr (fun r -> c (r @ [x]));;


// (ii)
(* 
    > revc [1;2;3;4;5;6] (fun v -> v);;
    val it: int list = [6; 5; 4; 3; 2; 1]

    > revc [1;2;3;4;5;6] (fun v -> v @ v);;
    val it: int list = [6; 5; 4; 3; 2; 1; 6; 5; 4; 3; 2; 1]
*)