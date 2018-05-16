using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Unity;
using Unity.Injection;
using Unity.Resolution;
using Unity.Lifetime;

namespace DependencyInjectionWithUnity
{
    class Program
    {
        static void Main(string[] args)
        {
            //-->CONFIGURING CONTAINER

            var container = new UnityContainer(); //we don't have to register shopper.  We do have to register Visa and MasterCard because they're both implementing ICreditCard, and it needs to be able to distinguish between the two.

            //registration methods to register types and pretty much do whatever you would need to do.  With Unity, if you needed to create a type in a certain way or to control that in a specific situation, I want to use this constrcutor and use x hard-coded value, etc.  You can also load stuff up from a config file. 


            container.RegisterType<ICreditCard, MasterCard>(); //basic registration.  
            //container.RegisterType<ICreditCard, MasterCard>(new ContainerControlledLifetimeManager()); //use this in order to create kind of a static object that is shared.  Only constructed one time.
            //container.RegisterType<ICreditCard, MasterCard>(new InjectionProperty("ChargeCount", 5));  //when we ask for an icreditcard, it's gonna give us a mc, then it's gonna do setter injection, setting the chargecount property to 5 to start off.
            //container.RegisterType<ICreditCard, MasterCard>("DefaultCard");//then when we resolve the type, we can pass the name and get this particular mapping.
            //container.RegisterType<ICreditCard, MasterCard>("BackupCard"); when we resolve, could ask for Default or Backup

            //To create an instance:

            //var card = new MasterCard();
            //container.RegisterInstance(card); //this is useful if you want to set stuff up ahead of time.


            //-->USING CONTAINER

            var shopper = container.Resolve<Shopper>();//Shopper doesn't have to be registered becasue, since there is only one and it's type is Shopper, it resolves it to shopper. The container is going to say, oh, shopper has a constructor, it's constructor takes an ICreditCard, therefore I need to provide one.  So it's going to look at it's dictionary and find that ICreditCard should be a master card, and it will return that.
            var shopper2 = container.Resolve<Shopper>(new ParameterOverride("creditCard", new Visa())); //Using resolution you can control exactly what happens when the dependency is resolved.  In this case, we should always get a visa, even though only mastercard is registered.
            shopper.Charge();
            shopper2.Charge();
            Console.WriteLine(shopper.ChargesForCurrentCard);




            Console.WriteLine(shopper.ChargesForCurrentCard);

            var shopper3 = container.Resolve<Shopper>();
            shopper3.Charge();
            Console.WriteLine(shopper.ChargesForCurrentCard); //I get one for each when it's a default container and 2 when it's set as a lifetime/singleton container.
            Console.Read();

            //var myClass = new object();

            Console.Read();

        }
    }

    public class Visa : ICreditCard
    {
        public string Charge()
        {
            return "Visa... Visa";
        }

        public int ChargeCount
        {
            get { return 0; }
        }
    }

    public class MasterCard : ICreditCard
    {
        public string Charge()
        {
            ChargeCount++;
            return "Charging with the MasterCard!";
        }

        public int ChargeCount { get; set; }
    }

    public interface ICreditCard
    {
        string Charge();
        int ChargeCount { get; }
    }

    public class Shopper
    {
        private readonly ICreditCard creditCard;

        public Shopper(ICreditCard creditCard)
        {
            
            this.creditCard = creditCard;
        }

        public int ChargesForCurrentCard
        {
            get { return creditCard.ChargeCount; }
        }

        public void Charge()
        {
            Console.WriteLine(creditCard.Charge());
        }
    }
}
