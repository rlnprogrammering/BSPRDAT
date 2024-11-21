# Exercise 10.1
(i)
• ADD, which adds two integers.

    Takes the two last items pushed to the stack and adds them together, 
    then removes the two variables used for the addition.

• CSTI i, which pushes integer constant i.

    Pushes integer constant i to the stack.

• NIL, which pushes a nil reference. What is the difference between NIL and CSTI 0?

    loads a nil referene (NIL is an empty address) while CSTI 0 is the integer constant 0.

• IFZERO a, which tests whether an integer is zero, or a reference is nil.

    checks if the top of stak is either zero or a nil reference, if is, then it jumps to 'a'.

• CONS

    Allocates two-word block on heap (a tuple), put reference to it on stack.

• CAR

    Access word 1 of block (fst argument of CONS tuple).

• SETCAR

    Set word 1 of block. Takes word on top of stack and sets the CAR for the CONS, then returns the CONS tuple.

(ii)
After applying Length, Color and Paint to ttttttttnnnnnnnnnnnnnnnnnnnnnngg (32-bit word)

    tttttttt (8): Block tag, always 0
    nnnnnnnnnnnnnnnnnnnnnn (22): Block length, here 2 (two words in this block)
    gg (2): Garbage Collector color


    The garbage collection bits gg, will be interpreted as colors:
    00 (white): May be collected
    01 (grey):  Not yet marked
    10 (black): Cannot be collected
    11 (blue):  On freelist, or is orphan block


(iii) When does the abstract machine, or more precisely, its instruction interpretation
loop, call the allocate(...) function? Is there any other interaction between the
abstract machine (also called the mutator) and the garbage collector?

    The allocate(...) function allocates from the freelist, every time cons(x,y) is called.


(iv) In what situation will the garbage collector’s collect(...) function be called?

    During allocate(...) if there is no free space, collect(...) does a garbage collection, so that allocate can try again.

# Exercise 10.2
Add a simple mark-sweep garbage collector to listmachine.c.

**Note:** Listmachine working directory we have run the code from:

`Assignment9/ListC/ListVM/ListVM`

How to run from this directory:

`$ ./listmachine ../../ex35.out`

Code:

```c
void markPhase(word s[], word sp) {
  printf("marking ...\n");
  for (word i = 0; i < sp; i++) {
    word* element = &s[i];
    Paint(element[0], Black); // Maybe?
    // printf("Stack element %lld: %ld\n", i, element);
  }
}

void sweepPhase() {
  printf("sweeping ...\n");
  word* current = heap;
  word** prevH = &heap;
  while (current != NULL) {
    if (Color(current[0]) == White) {
        // Add to freelist
        prevH = (word**)&current[1];    // Link the previous block to the next block
        current[1] = (word)freelist;  // Link the current block to the freelist
        freelist = current;     // Update the freelist to point to the current block
    } else if (Color(current[0]) == Black) {
        // Recolor to white
        Paint(current[0], White);
        prevH = (word**)&current[1];  // Move to the next block
    }
    current = (word*)*prevH;     // Move to the next block
  }
}
```

    

# Exercise 10.3
We spent many hours on this exercise without any breakthrough. PLEASE HELP

What is supposed to be the correct solution? 
We had trouble accessing the previous Header, and how to merge it into one block...
