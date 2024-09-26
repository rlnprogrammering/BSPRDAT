(* Exercise 5.1 A *)
let merge (list1, list2) : int list =
    list1 @ list2 |> List.sort;;

let ex1 = merge ([3;5;12], [2;3;4;7]);;

(* Exercise 5.7 *)