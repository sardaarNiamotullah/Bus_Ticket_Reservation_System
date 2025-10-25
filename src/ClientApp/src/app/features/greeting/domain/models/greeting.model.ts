export interface Greeting {
  message: string;
  timestamp: Date;
}

export class GreetingEntity implements Greeting {
  constructor(
    public message: string,
    public timestamp: Date = new Date()
  ) {}

  static createHelloWorld(): GreetingEntity {
    return new GreetingEntity('Hello World');
  }
}