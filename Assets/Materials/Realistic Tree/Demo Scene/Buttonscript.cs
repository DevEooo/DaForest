using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttonscript : MonoBehaviour
{
    //MODELS
    public GameObject ash1;
    public GameObject ash2;
    public GameObject ash3;
    public GameObject ash4;
    public GameObject ash5;
    public GameObject ash6;
    public GameObject ash7;
    public GameObject ash8;
    public GameObject ash9;

    public GameObject chestnut1;
    public GameObject chestnut2;
    public GameObject chestnut3;
    public GameObject chestnut4;
    public GameObject chestnut5;
    public GameObject chestnut6;
    public GameObject chestnut7;
    public GameObject chestnut8;

    public GameObject weeping1;
    public GameObject weeping2;
    public GameObject weeping3;
    public GameObject weeping4;
    public GameObject weeping5;
    public GameObject weeping6;
    public GameObject weeping7;
    public GameObject weeping8;

    public GameObject spruce1;
    public GameObject spruce2;
    public GameObject spruce3;
    public GameObject spruce4;
    public GameObject spruce5;
    public GameObject spruce6;
    public GameObject spruce7;
    public GameObject spruce8;
    public GameObject spruce9;
    public GameObject spruce10;
    public GameObject spruce11;
    public GameObject spruce12;
    public GameObject spruce13;

    public GameObject birch1;
    public GameObject birch2;
    public GameObject birch3;
    public GameObject birch4;
    public GameObject birch5;
    public GameObject birch6;
    public GameObject birch7;
    public GameObject birch8;
    public GameObject birch9;
    public GameObject birch10;
    public GameObject birch11;
    public GameObject birch12;
    public GameObject birch13;
    

    

    //BUTTONS

    public GameObject ButtonASH1;
    public GameObject ButtonASH2;
    public GameObject ButtonASH3;
    public GameObject ButtonASH4;
    public GameObject ButtonASH5;
    public GameObject ButtonASH6;
    public GameObject ButtonASH7;
    public GameObject ButtonASH8;
    public GameObject ButtonASH9;

    public GameObject ButtonChestnut1;
    public GameObject ButtonChestnut2;
    public GameObject ButtonChestnut3;
    public GameObject ButtonChestnut4;
    public GameObject ButtonChestnut5;
    public GameObject ButtonChestnut6;
    public GameObject ButtonChestnut7;
    public GameObject ButtonChestnut8;

    public GameObject ButtonWeeping1;
    public GameObject ButtonWeeping2;
    public GameObject ButtonWeeping3;
    public GameObject ButtonWeeping4;
    public GameObject ButtonWeeping5;
    public GameObject ButtonWeeping6;
    public GameObject ButtonWeeping7;
    public GameObject ButtonWeeping8;

    public GameObject ButtonSpruce1;
    public GameObject ButtonSpruce2;
    public GameObject ButtonSpruce3;
    public GameObject ButtonSpruce4;
    public GameObject ButtonSpruce5;
    public GameObject ButtonSpruce6;
    public GameObject ButtonSpruce7;
    public GameObject ButtonSpruce8;
    public GameObject ButtonSpruce9;
    public GameObject ButtonSpruce10;
    public GameObject ButtonSpruce11;
    public GameObject ButtonSpruce12;
    public GameObject ButtonSpruce13;

    public GameObject ButtonBirch1;
    public GameObject ButtonBirch2;
    public GameObject ButtonBirch3;
    public GameObject ButtonBirch4;
    public GameObject ButtonBirch5;
    public GameObject ButtonBirch6;
    public GameObject ButtonBirch7;
    public GameObject ButtonBirch8;
    public GameObject ButtonBirch9;
    public GameObject ButtonBirch10;
    public GameObject ButtonBirch11;
    public GameObject ButtonBirch12;
    public GameObject ButtonBirch13;







    public GameObject AshTrees;
    public GameObject BirchTrees;
    public GameObject ChestnutTrees;
    public GameObject SpruceTrees;
    public GameObject WeepingWillowTrees;
    public GameObject GoBack;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ashtree1()
    {
        if (ash1.activeInHierarchy == true)
            ash1.SetActive(false);
        else
            ash1.SetActive(true);
            ash6.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree2()
    {
        if (ash2.activeInHierarchy == true)
            ash2.SetActive(false);
        else
            ash2.SetActive(true);
            ash1.SetActive(false);
            ash6.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree3()
    {
        if (ash3.activeInHierarchy == true)
            ash3.SetActive(false);
        else
            ash3.SetActive(true);
            ash1.SetActive(false);
            ash6.SetActive(false);
            ash2.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree4()
    {
        if (ash4.activeInHierarchy == true)
            ash4.SetActive(false);
        else
            ash4.SetActive(true);
            ash1.SetActive(false);
            ash6.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash5.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree5()
    {
        if (ash5.activeInHierarchy == true)
            ash5.SetActive(false);
        else
            ash5.SetActive(true);
            ash1.SetActive(false);
            ash6.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree6()
    {
        if (ash6.activeInHierarchy == true)
            ash6.SetActive(false);
        else
            ash6.SetActive(true);
            ash1.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree7()
    {
        if (ash7.activeInHierarchy == true)
            ash7.SetActive(false);
        else
            ash7.SetActive(true);
            ash1.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash6.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree8()
    {
        if (ash8.activeInHierarchy == true)
            ash8.SetActive(false);
        else
            ash8.SetActive(true);
            ash1.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash6.SetActive(false);
            ash7.SetActive(false);
            ash9.SetActive(false);
    }

    public void ashtree9()
    {
        if (ash9.activeInHierarchy == true)
            ash9.SetActive(false);
        else
            ash9.SetActive(true);
            ash1.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash6.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
    }


    public void chestnuttree1()
    {
        if (chestnut1.activeInHierarchy == true)
            chestnut1.SetActive(false);
        else
            chestnut1.SetActive(true);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);
    }


    public void chestnuttree2()
    {
        if (chestnut2.activeInHierarchy == true)
            chestnut2.SetActive(false);
        else
            chestnut2.SetActive(true);
            chestnut1.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);
    }


    public void chestnuttree3()
    {
        if (chestnut3.activeInHierarchy == true)
            chestnut3.SetActive(false);
        else
            chestnut3.SetActive(true);
            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);
    }

    public void chestnuttree4()
    {
        if (chestnut4.activeInHierarchy == true)
            chestnut4.SetActive(false);
        else
            chestnut4.SetActive(true);
            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);
    }


    public void chestnuttree5()
    {
        if (chestnut5.activeInHierarchy == true)
            chestnut5.SetActive(false);
        else
            chestnut5.SetActive(true);
            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);
    }

    public void chestnuttree6()
    {
        if (chestnut6.activeInHierarchy == true)
            chestnut6.SetActive(false);
        else
            chestnut6.SetActive(true);
            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);
    }

    public void chestnuttree7()
    {
        if (chestnut7.activeInHierarchy == true)
            chestnut7.SetActive(false);
        else
            chestnut7.SetActive(true);
            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut8.SetActive(false);
    }

    public void chestnuttree8()
    {
        if (chestnut8.activeInHierarchy == true)
            chestnut8.SetActive(false);
        else
            chestnut8.SetActive(true);
            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
    }


    public void weepingtree1()
    {
        if (weeping1.activeInHierarchy == true)
            weeping1.SetActive(false);
        else
            weeping1.SetActive(true);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);           
    }

    public void weepingtree2()
    {
        if (weeping2.activeInHierarchy == true)
            weeping2.SetActive(false);
        else
            weeping2.SetActive(true);
            weeping1.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);           
    }

    public void weepingtree3()
    {
        if (weeping3.activeInHierarchy == true)
            weeping3.SetActive(false);
        else
            weeping3.SetActive(true);
            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);           
    }

    public void weepingtree4()
    {
        if (weeping4.activeInHierarchy == true)
            weeping4.SetActive(false);
        else
            weeping4.SetActive(true);
            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);           
    }


    public void weepingtree5()
    {
        if (weeping5.activeInHierarchy == true)
            weeping5.SetActive(false);
        else
            weeping5.SetActive(true);
            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);           
    }


    public void weepingtree6()
    {
        if (weeping6.activeInHierarchy == true)
            weeping6.SetActive(false);
        else
            weeping6.SetActive(true);
            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);           
    }

    public void weepingtree7()
    {
        if (weeping7.activeInHierarchy == true)
            weeping7.SetActive(false);
        else
            weeping7.SetActive(true);
            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping8.SetActive(false);           
    }

    public void weepingtree8()
    {
        if (weeping8.activeInHierarchy == true)
            weeping8.SetActive(false);
        else
            weeping8.SetActive(true);
            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);           
    }


    public void sprucetree1()
    {
        if (spruce1.activeInHierarchy == true)
            spruce1.SetActive(false);
        else
            spruce1.SetActive(true);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree2()
    {
        if (spruce2.activeInHierarchy == true)
            spruce2.SetActive(false);
        else
            spruce2.SetActive(true);
            spruce1.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree3()
    {
        if (spruce3.activeInHierarchy == true)
            spruce3.SetActive(false);
        else
            spruce3.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree4()
    {
        if (spruce4.activeInHierarchy == true)
            spruce4.SetActive(false);
        else
            spruce4.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree5()
    {
        if (spruce5.activeInHierarchy == true)
            spruce5.SetActive(false);
        else
            spruce5.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree6()
    {
        if (spruce6.activeInHierarchy == true)
            spruce6.SetActive(false);
        else
            spruce6.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }


    public void sprucetree7()
    {
        if (spruce7.activeInHierarchy == true)
            spruce7.SetActive(false);
        else
            spruce7.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }


    public void sprucetree8()
    {
        if (spruce8.activeInHierarchy == true)
            spruce8.SetActive(false);
        else
            spruce8.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree9()
    {
        if (spruce9.activeInHierarchy == true)
            spruce9.SetActive(false);
        else
            spruce9.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }


    public void sprucetree10()
    {
        if (spruce10.activeInHierarchy == true)
            spruce10.SetActive(false);
        else
            spruce10.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree11()
    {
        if (spruce11.activeInHierarchy == true)
            spruce11.SetActive(false);
        else
            spruce11.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree12()
    {
        if (spruce12.activeInHierarchy == true)
            spruce12.SetActive(false);
        else
            spruce12.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce13.SetActive(false);
                       
    }

    public void sprucetree13()
    {
        if (spruce13.activeInHierarchy == true)
            spruce13.SetActive(false);
        else
            spruce13.SetActive(true);
            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
                       
    }

    public void birchtree1()
    {
        if (birch1.activeInHierarchy == true)
            birch1.SetActive(false);
        else
            birch1.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree2()
    {
        if (birch2.activeInHierarchy == true)
            birch2.SetActive(false);
        else
            birch2.SetActive(true);
            birch1.SetActive(false);
            birch3.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree3()
    {
        if (birch3.activeInHierarchy == true)
            birch3.SetActive(false);
        else
            birch3.SetActive(true);
            birch2.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree4()
    {
        if (birch4.activeInHierarchy == true)
            birch4.SetActive(false);
        else
            birch4.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree5()
    {
        if (birch5.activeInHierarchy == true)
            birch5.SetActive(false);
        else
            birch5.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree6()
    {
        if (birch6.activeInHierarchy == true)
            birch6.SetActive(false);
        else
            birch6.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree7()
    {
        if (birch7.activeInHierarchy == true)
            birch7.SetActive(false);
        else
            birch7.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree8()
    {
        if (birch8.activeInHierarchy == true)
            birch8.SetActive(false);
        else
            birch8.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree9()
    {
        if (birch9.activeInHierarchy == true)
            birch9.SetActive(false);
        else
            birch9.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree10()
    {
        if (birch10.activeInHierarchy == true)
            birch10.SetActive(false);
        else
            birch10.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree11()
    {
        if (birch11.activeInHierarchy == true)
            birch11.SetActive(false);
        else
            birch11.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);                  
    }


    public void birchtree12()
    {
        if (birch12.activeInHierarchy == true)
            birch12.SetActive(false);
        else
            birch12.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch13.SetActive(false);                  
    }

    public void birchtree13()
    {
        if (birch13.activeInHierarchy == true)
            birch13.SetActive(false);
        else
            birch13.SetActive(true);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch1.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);                  
    }


    //MENU NAVIGATION
    public void AshTreesMenu()
    {
        if (AshTrees.activeInHierarchy == true)
            AshTrees.SetActive(false);
        else
            AshTrees.SetActive(true);
            BirchTrees.SetActive(false);
            ChestnutTrees.SetActive(false);
            SpruceTrees.SetActive(false);
            WeepingWillowTrees.SetActive(false);
            GoBack.SetActive(true);
            ButtonASH1.SetActive(true);
            ButtonASH2.SetActive(true);
            ButtonASH3.SetActive(true);
            ButtonASH4.SetActive(true);
            ButtonASH5.SetActive(true);
            ButtonASH6.SetActive(true);
            ButtonASH7.SetActive(true);
            ButtonASH8.SetActive(true);
            ButtonASH9.SetActive(true);
    }

    public void ChestnutTreesMenu()
    {
        if (ChestnutTrees.activeInHierarchy == true)
            ChestnutTrees.SetActive(false);
        else
            ChestnutTrees.SetActive(true);
            BirchTrees.SetActive(false);
            AshTrees.SetActive(false);
            SpruceTrees.SetActive(false);
            WeepingWillowTrees.SetActive(false);
            GoBack.SetActive(true);
            ButtonChestnut1.SetActive(true);
            ButtonChestnut2.SetActive(true);
            ButtonChestnut3.SetActive(true);
            ButtonChestnut4.SetActive(true);
            ButtonChestnut5.SetActive(true);
            ButtonChestnut6.SetActive(true);
            ButtonChestnut7.SetActive(true);
            ButtonChestnut8.SetActive(true);

            
    }

    public void WeepingTreesMenu()
    {
        if (WeepingWillowTrees.activeInHierarchy == true)
            WeepingWillowTrees.SetActive(false);
        else
            WeepingWillowTrees.SetActive(true);
            BirchTrees.SetActive(false);
            AshTrees.SetActive(false);
            SpruceTrees.SetActive(false);
            ChestnutTrees.SetActive(false);
            GoBack.SetActive(true);
            ButtonWeeping1.SetActive(true);
            ButtonWeeping2.SetActive(true);
            ButtonWeeping3.SetActive(true);
            ButtonWeeping4.SetActive(true);
            ButtonWeeping5.SetActive(true);
            ButtonWeeping6.SetActive(true);
            ButtonWeeping7.SetActive(true);
            ButtonWeeping8.SetActive(true);
            

            
    }


    public void SpruceTreesMenu()
    {
        if (SpruceTrees.activeInHierarchy == true)
            SpruceTrees.SetActive(false);
        else
            SpruceTrees.SetActive(true);
            BirchTrees.SetActive(false);
            AshTrees.SetActive(false);
            WeepingWillowTrees.SetActive(false);
            ChestnutTrees.SetActive(false);
            GoBack.SetActive(true);
            ButtonSpruce1.SetActive(true);
            ButtonSpruce2.SetActive(true);
            ButtonSpruce3.SetActive(true);
            ButtonSpruce4.SetActive(true);
            ButtonSpruce5.SetActive(true);
            ButtonSpruce6.SetActive(true);
            ButtonSpruce7.SetActive(true);
            ButtonSpruce8.SetActive(true);
            ButtonSpruce9.SetActive(true);
            ButtonSpruce10.SetActive(true);
            ButtonSpruce11.SetActive(true);
            ButtonSpruce12.SetActive(true);
            ButtonSpruce13.SetActive(true);
            
            

            
    }


    public void BirchTreesMenu()
    {
        if (BirchTrees.activeInHierarchy == true)
            BirchTrees.SetActive(false);
        else
            BirchTrees.SetActive(true);
            SpruceTrees.SetActive(false);
            AshTrees.SetActive(false);
            WeepingWillowTrees.SetActive(false);
            ChestnutTrees.SetActive(false);
            GoBack.SetActive(true);
            ButtonBirch1.SetActive(true);
            ButtonBirch2.SetActive(true);
            ButtonBirch3.SetActive(true);
            ButtonBirch4.SetActive(true);
            ButtonBirch5.SetActive(true);
            ButtonBirch6.SetActive(true);
            ButtonBirch7.SetActive(true);
            ButtonBirch8.SetActive(true);
            ButtonBirch9.SetActive(true);
            ButtonBirch10.SetActive(true);
            ButtonBirch11.SetActive(true);
            ButtonBirch12.SetActive(true);
            ButtonBirch13.SetActive(true);
            
            
            

            
    }





    
    public void back()
    {
        if (GoBack.activeInHierarchy == true)
            GoBack.SetActive(false);
        else
            GoBack.SetActive(true);
            AshTrees.SetActive(true);
            BirchTrees.SetActive(true);
            ChestnutTrees.SetActive(true);
            SpruceTrees.SetActive(true);
            WeepingWillowTrees.SetActive(true);

            //ButtonsSingleTrees deactivated
            ButtonASH1.SetActive(false);
            ButtonASH2.SetActive(false);
            ButtonASH3.SetActive(false);
            ButtonASH4.SetActive(false);
            ButtonASH5.SetActive(false);
            ButtonASH6.SetActive(false);
            ButtonASH7.SetActive(false);
            ButtonASH8.SetActive(false);
            ButtonASH9.SetActive(false);

            ButtonChestnut1.SetActive(false);
            ButtonChestnut2.SetActive(false);
            ButtonChestnut3.SetActive(false);
            ButtonChestnut4.SetActive(false);
            ButtonChestnut5.SetActive(false);
            ButtonChestnut6.SetActive(false);
            ButtonChestnut7.SetActive(false);
            ButtonChestnut8.SetActive(false);

            ButtonWeeping1.SetActive(false);
            ButtonWeeping2.SetActive(false);
            ButtonWeeping3.SetActive(false);
            ButtonWeeping4.SetActive(false);
            ButtonWeeping5.SetActive(false);
            ButtonWeeping6.SetActive(false);
            ButtonWeeping7.SetActive(false);
            ButtonWeeping8.SetActive(false);

            ButtonSpruce1.SetActive(false);
            ButtonSpruce2.SetActive(false);
            ButtonSpruce3.SetActive(false);
            ButtonSpruce4.SetActive(false);
            ButtonSpruce5.SetActive(false);
            ButtonSpruce6.SetActive(false);
            ButtonSpruce7.SetActive(false);
            ButtonSpruce8.SetActive(false);
            ButtonSpruce9.SetActive(false);
            ButtonSpruce10.SetActive(false);
            ButtonSpruce11.SetActive(false);
            ButtonSpruce12.SetActive(false);
            ButtonSpruce13.SetActive(false);

            ButtonBirch1.SetActive(false);
            ButtonBirch2.SetActive(false);
            ButtonBirch3.SetActive(false);
            ButtonBirch4.SetActive(false);
            ButtonBirch5.SetActive(false);
            ButtonBirch6.SetActive(false);
            ButtonBirch7.SetActive(false);
            ButtonBirch8.SetActive(false);
            ButtonBirch9.SetActive(false);
            ButtonBirch10.SetActive(false);
            ButtonBirch11.SetActive(false);
            ButtonBirch12.SetActive(false);
            ButtonBirch13.SetActive(false);



            //Models

            ash1.SetActive(false);
            ash2.SetActive(false);
            ash3.SetActive(false);
            ash4.SetActive(false);
            ash5.SetActive(false);
            ash6.SetActive(false);
            ash7.SetActive(false);
            ash8.SetActive(false);
            ash9.SetActive(false);

            chestnut1.SetActive(false);
            chestnut2.SetActive(false);
            chestnut3.SetActive(false);
            chestnut4.SetActive(false);
            chestnut5.SetActive(false);
            chestnut6.SetActive(false);
            chestnut7.SetActive(false);
            chestnut8.SetActive(false);

            weeping1.SetActive(false);
            weeping2.SetActive(false);
            weeping3.SetActive(false);
            weeping4.SetActive(false);
            weeping5.SetActive(false);
            weeping6.SetActive(false);
            weeping7.SetActive(false);
            weeping8.SetActive(false);

            spruce1.SetActive(false);
            spruce2.SetActive(false);
            spruce3.SetActive(false);
            spruce4.SetActive(false);
            spruce5.SetActive(false);
            spruce6.SetActive(false);
            spruce7.SetActive(false);
            spruce8.SetActive(false);
            spruce9.SetActive(false);
            spruce10.SetActive(false);
            spruce11.SetActive(false);
            spruce12.SetActive(false);
            spruce13.SetActive(false);

            birch1.SetActive(false);
            birch2.SetActive(false);
            birch3.SetActive(false);
            birch4.SetActive(false);
            birch5.SetActive(false);
            birch6.SetActive(false);
            birch7.SetActive(false);
            birch8.SetActive(false);
            birch9.SetActive(false);
            birch10.SetActive(false);
            birch11.SetActive(false);
            birch12.SetActive(false);
            birch13.SetActive(false);




    }

   

}
