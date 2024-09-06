// File Intro/SimpleExpr.java
// Java representation of expressions as in lecture 1
// sestoft@itu.dk * 2010-08-29

import java.util.Map;
import java.util.HashMap;

abstract class Expr { 
  abstract public Expr simplify();
  abstract public int eval(Map<String,Integer> env);
}

class CstI extends Expr { 
  protected final int i;

  public CstI(int i) { 
    this.i = i; 
  }

  public int eval(Map<String,Integer> env) {
    return i;
  }

  public String toString(){
    return "" + i;
  }
  
    @Override
    public Expr simplify() {
        return this; // Variables cannot be simplified further
    }
}

class Var extends Expr { 
  protected final String name;

  public Var(String name) { 
    this.name = name; 
  }

  public int eval(Map<String,Integer> env) {
    return env.get(name);
  }
  
  @Override
  public Expr simplify() {
      return this; // Variables cannot be simplified further
  }

  public String toString(){
    return name;
  }

}

class Prim extends Expr { //  We expect this to be Binop according to the task
  protected final String oper;
  protected final Expr e1, e2;

  public Prim(String oper, Expr e1, Expr e2) { 
    this.oper = oper; this.e1 = e1; this.e2 = e2;
  }

  public int eval(Map<String,Integer> env) {
    if (oper.equals("+"))
      return e1.eval(env) + e2.eval(env);
    else if (oper.equals("*"))
      return e1.eval(env) * e2.eval(env);
    else if (oper.equals("-"))
      return e1.eval(env) - e2.eval(env);
    else
      throw new RuntimeException("unknown primitive");
  }
  
  @Override
  public Expr simplify() {
    if (oper.equals("+")) {
        if (e1.simplify().toString().equals("0")) { return e2.simplify(); }
        if (e2.simplify().toString().equals("0")) { return e1.simplify(); }
        return new Add(e1.simplify(), e2.simplify());
    } else if (oper.equals("*")){
      if (e1.simplify().toString().equals("0")) { return new CstI(0); }
      if (e2.simplify().toString().equals("0")) { return new CstI(0); }
      if (e1.simplify().toString().equals("1")) { return e2.simplify(); }
      if (e2.simplify().toString().equals("1")) { return e1.simplify(); }
      return new Mul(e1.simplify(), e2.simplify());
    } else if (oper.equals("-")) {
      if (e1.simplify().toString().equals("0")) { return e2.simplify(); }
      if (e2.simplify().toString().equals("0")) { return e1.simplify(); }
      if (e1.simplify().toString().equals(e2.toString())) { return new CstI(0); }
      return new Sub(e1.simplify(), e2.simplify());
    } else {
      throw new RuntimeException("unknown primitive");
    }
  }


 public String toString() {
  return e1 + " " + oper + " " + e2;
 }
}

class Add extends Prim {
 public Add(Expr e1, Expr e2) {
  super("+", e1, e2);
 }
}

class Sub extends Prim {
 public Sub(Expr e1, Expr e2) {
  super("-", e1, e2);
 }
}

class Mul extends Prim {
 public Mul(Expr e1, Expr e2) {
  super("*", e1, e2);
 }
}

public class SimpleExpr {
  public static void main(String[] args) {
    Expr e1 = new CstI(17);
    Expr e2 = new Prim("+", new CstI(3), new Var("a"));
    Expr e3 = new Prim("+", new Prim("*", new Var("b"), new CstI(9)), 
		            new Var("a"));
    Map<String,Integer> env0 = new HashMap<String,Integer>();
    env0.put("a", 3);
    env0.put("c", 78);
    env0.put("baf", 666);
    env0.put("b", 111);

    System.out.println(e1.eval(env0));
    System.out.println(e2.eval(env0));
    System.out.println(e3.eval(env0));

    // Excercise 1.4 (i):
    Expr e4 = new Add(new CstI(0), new Var("z"));

    // Excercise 1.4 (ii):
    Expr e5 = new Sub(new CstI(17), new Add(new CstI(12), new Var("x")));
    Expr e6 = new Mul(new CstI(17), new Var("z"));
    Expr e7 = new Mul(new CstI(17), new Sub(new CstI(32), new CstI(32)));

    System.out.println("e4 toString: " + e4.toString());
    System.out.println("e5 toString: " + e5.toString());
    System.out.println("e6 toString: " + e6.toString());
    System.out.println("e7 toString: " + e7.toString());
    
    // Excercise 1.4 (iv):
    Expr e8 = new Sub(new Var("z"), new Var("z"));
    Expr e9 = new Mul(new Var("x"), new CstI(1));
    Expr e10 = new Add(new Mul(new CstI(0), new Add(new CstI(5), new CstI(3))), new CstI(3)); // (0 * (5 + 3)) + 3
    Expr e11 = new Sub(new Add(new Mul(new CstI(10), new CstI(1)), new CstI(5)), new CstI(15)); // ((10 * 1) + 5) - 15
    Expr e12 = new Mul(new Add(new CstI(2), new Mul(new CstI(3), new CstI(1))), new CstI(4)); // (2 + (3 * 1)) * 4


    System.out.println("Simplify e4: " + e4.simplify());
    System.out.println("simplify e7: " + e7.simplify());
    System.out.println("simplify e8: " + e8.simplify());
    System.out.println("simplify e9: " + e9.simplify());
    System.out.println("simplify e10: " + e10.simplify());
    System.out.println("simplify e11: " + e11.simplify());
    System.out.println("simplify e12: " + e12.simplify());
  }
}
