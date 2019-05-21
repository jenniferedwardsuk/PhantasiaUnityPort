//import java.awt.JavaPanel;
//import java.awt.JavaLabel;
//import java.awt.Canvas;
//import java.awt.Color;
//import java.awt.BorderLayout;
//import java.awt.FlowLayout;
//import java.awt.JavaGridLayout;
//import java.awt.GridBagLayout;
//import java.awt.GridBagConstraints;
//import java.awt.MediaTracker;
//import java.io.DataInputStream;
//import java.awt.Font;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class statusPne : JavaPanel
{

    private pClient parent = null;

    GridBagLayout paneLayout = new GridBagLayout();
    GridBagLayout energyLayout = new GridBagLayout();
    GridBagLayout strengthLayout = new GridBagLayout();
    GridBagLayout quicknessLayout = new GridBagLayout();

    private JavaPanel titlePane;
    private headPne titleCanvas;

    private JavaPanel statsPane;

    private JavaPanel energyPane;
    private titlePne energyJavaLabel;
    private barGraph energyGraph;
    public imagePne shieldImage;
    private numberPne shieldJavaLabel;

    private JavaPanel strengthPane;
    private titlePne strengthJavaLabel;
    private barGraph strengthGraph;
    private imagePne swordImage;
    private numberPne swordJavaLabel;

    private JavaPanel quicknessPane;
    private titlePne quicknessJavaLabel;
    private barGraph quicknessGraph;
    private imagePne quicksilverImage;
    private numberPne quicksilverJavaLabel;

    private JavaPanel treasurePane;

    private JavaPanel manaPane;
    private imagePne manaImage;
    private numberPne manaJavaLabel;

    private JavaPanel levelPane;
    private imagePne levelImage;
    private numberPne levelJavaLabel;

    private JavaPanel goldPane;
    private imagePne goldImage;
    private numberPne goldJavaLabel;

    private JavaPanel gemsPane;
    private imagePne gemsImage;
    private numberPne gemsJavaLabel;

    private JavaPanel equipmentPane;

    private imagePne cloakImage;
    private imagePne blessingImage;
    private imagePne crownImage;
    private imagePne palantirImage;
    private imagePne ringImage;
    private imagePne virginImage;

    private string name;          /* player name */
    private string location;	/* player location */
    public statusPne(pClient c) : base("StatusPne", true)
    {
        titlePane = new JavaPanel("Title", false, true); //todo: this is disconnected from titleCanvas
        titleCanvas = new headPne("Welcome to Phantasia!");

        statsPane = new JavaPanel("Stats", false, true, true);

        energyPane = new JavaPanel("Energy", false, true, false);
        energyJavaLabel = new titlePne("Energy", false);
        energyGraph = new barGraph();
        shieldImage = null;
        shieldJavaLabel = new numberPne("0", "Shield");

        strengthPane = new JavaPanel("Strength", false, true, true);
        strengthJavaLabel = new titlePne("Strength", false);
        strengthGraph = new barGraph();
        swordImage = null;
        swordJavaLabel = new numberPne("0", "Sword");

        quicknessPane = new JavaPanel("Quickness", false, true, true);
        quicknessJavaLabel = new titlePne("Speed", false);
        quicknessGraph = new barGraph();
        quicksilverImage = null;
        quicksilverJavaLabel = new numberPne("0", "Quicksilver");

        treasurePane = new JavaPanel("Treasure", false, true, true);

        manaPane = new JavaPanel("Mana", false, true, false);
        manaImage = null;
        manaJavaLabel = new numberPne("0", "Mana");

        levelPane = new JavaPanel("Level", false, true, true);
        levelImage = null;
        levelJavaLabel = new numberPne("0", "Level");

        goldPane = new JavaPanel("Gold", false, true, true);
        goldImage = null;
        goldJavaLabel = new numberPne("0", "Gold");

        gemsPane = new JavaPanel("Gems", false, true, true);
        gemsImage = null;
        gemsJavaLabel = new numberPne("0", "Gems");

        equipmentPane = new JavaPanel("Equipment", false, true, true);

        cloakImage = null;
        blessingImage = null;
        crownImage = null;
        palantirImage = null;
        ringImage = null;
        virginImage = null;

        GridBagConstraints constraints = new GridBagConstraints();

        parent = c;

        shieldImage = new imagePne(parent, "Shield");
        swordImage = new imagePne(parent, "Sword");
        quicksilverImage = new imagePne(parent, "Quicksilver");
        manaImage = new imagePne(parent, "Mana");
        levelImage = new imagePne(parent, "Level");
        goldImage = new imagePne(parent, "Gold");
        gemsImage = new imagePne(parent, "Gems");
        cloakImage = new imagePne(parent, "Cloak");
        blessingImage = new imagePne(parent, "Blessing");
        crownImage = new imagePne(parent, "Crown");
        palantirImage = new imagePne(parent, "Palantir");
        ringImage = new imagePne(parent, "Ring");
        virginImage = new imagePne(parent, "Virgin");

        constraints.insets.top = 2;
        constraints.insets.bottom = 2;
        constraints.insets.left = 2;
        constraints.insets.right = 2;

        setBackground(backgroundColor);


        /* START MAIN PANE LAYOUT */
        //setLayout(paneLayout);

        init(constraints);
    }

    //internal void init(GridBagConstraints constraints)
    //{
    //    UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_init, constraints);
    //}
    internal void init(GridBagConstraints constraints)
    {
        constraints.gridx = 1;
        constraints.gridy = 0;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.HORIZONTAL;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(titleCanvas, constraints);
        add(titleCanvas);

        constraints.gridx = 0;
        constraints.gridy = 0;
        constraints.gridwidth = 1;
        constraints.gridheight = 3;
        constraints.weightx = 1;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.HORIZONTAL;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(statsPane, constraints);
        add(statsPane);

        constraints.gridx = 1;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.HORIZONTAL;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(treasurePane, constraints);
        add(treasurePane);

        constraints.gridx = 1;
        constraints.gridy = 2;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 1;
        constraints.fill = GridBagConstraints.BOTH;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(equipmentPane, constraints);
        equipmentPane.setBackground(backgroundColor);
        add(equipmentPane);

        setLayout(paneLayout);
        /* END MAIN PANE LAYOUT */

        /* START STATS PANE LAYOUT */
        //statsPane.setLayout(new JavaGridLayout(3, 1, 2, 4));
        statsPane.setBackground(backgroundColor);
        statsPane.add(energyPane);
        statsPane.add(strengthPane);
        statsPane.add(quicknessPane);
        statsPane.setLayout(new JavaGridLayout(3, 1, 2, 4));

        //energyPane.setLayout(energyLayout);
        energyPane.setBackground(highlightColor);
        constraints.insets.top = 0;
        constraints.insets.bottom = 0;
        constraints.insets.left = 0;
        constraints.insets.right = 0;

        constraints.gridx = 0;
        constraints.gridy = 0;
        constraints.gridwidth = 7;// 3;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.HORIZONTAL;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(energyJavaLabel, constraints);
        energyJavaLabel.setSize(16, 16);
        energyPane.add(energyJavaLabel);

        constraints.gridx = 0;
        constraints.gridy = 1;
        constraints.gridwidth = 5;// 1;
        constraints.gridheight = 1;
        constraints.weightx = 1;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.BOTH;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(energyGraph, constraints);
        energyPane.add(energyGraph);

        constraints.gridx = 5;// 1;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.insets.left = 4;
        constraints.insets.right = 2;
        constraints.fill = GridBagConstraints.NONE;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(shieldImage, constraints);
        shieldImage.setSize(18, 18);
        energyPane.add(shieldImage);

        constraints.gridx = 6;// 2;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.insets.left = 0;
        constraints.insets.right = 0;
        constraints.fill = GridBagConstraints.NONE;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(shieldJavaLabel, constraints);
        energyPane.add(shieldJavaLabel);

        energyPane.setLayout(energyLayout);

        //strengthPane.setLayout(strengthLayout);
        strengthPane.setBackground(highlightColor);

        constraints.gridx = 0;
        constraints.gridy = 0;
        constraints.gridwidth = 7;// 3;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.HORIZONTAL;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(strengthJavaLabel, constraints);
        strengthJavaLabel.setSize(16, 16);
        strengthPane.add(strengthJavaLabel);

        constraints.gridx = 0;
        constraints.gridy = 1;
        constraints.gridwidth = 5;// 1;
        constraints.gridheight = 1;
        constraints.weightx = 1;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.BOTH;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(strengthGraph, constraints);
        strengthPane.add(strengthGraph);

        constraints.gridx = 5;// 1;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.insets.left = 4;
        constraints.insets.right = 2;
        constraints.fill = GridBagConstraints.NONE;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(swordImage, constraints);
        swordImage.setSize(18, 18);
        strengthPane.add(swordImage);

        constraints.gridx = 6;// 2;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.insets.left = 0;
        constraints.insets.right = 0;
        constraints.fill = GridBagConstraints.NONE;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(swordJavaLabel, constraints);
        strengthPane.add(swordJavaLabel);

        strengthPane.setLayout(strengthLayout);

        //quicknessPane.setLayout(quicknessLayout);
        quicknessPane.setBackground(highlightColor);

        constraints.gridx = 0;
        constraints.gridy = 0;
        constraints.gridwidth = 7;// 3;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.HORIZONTAL;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(quicknessJavaLabel, constraints);
        quicknessJavaLabel.setSize(16, 16);
        quicknessPane.add(quicknessJavaLabel);

        constraints.gridx = 0;
        constraints.gridy = 1;
        constraints.gridwidth = 5;// 1;
        constraints.gridheight = 1;
        constraints.weightx = 1;
        constraints.weighty = 0;
        constraints.fill = GridBagConstraints.BOTH;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(quicknessGraph, constraints);
        quicknessPane.add(quicknessGraph);

        constraints.gridx = 5;// 1;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.insets.left = 4;
        constraints.insets.right = 2;
        constraints.fill = GridBagConstraints.NONE;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(quicksilverImage, constraints);
        quicksilverImage.setSize(18, 18);
        quicknessPane.add(quicksilverImage);

        constraints.gridx = 6;// 2;
        constraints.gridy = 1;
        constraints.gridwidth = 1;
        constraints.gridheight = 1;
        constraints.weightx = 0;
        constraints.weighty = 0;
        constraints.insets.left = 0;
        constraints.insets.right = 0;
        constraints.fill = GridBagConstraints.NONE;
        constraints.anchor = GridBagConstraints.CENTER;
        GridBagLayout.setConstraints(quicksilverJavaLabel, constraints);
        quicknessPane.add(quicksilverJavaLabel);

        quicknessPane.setLayout(quicknessLayout);

        /* END STATS PANE LAYOUT */

        /* BEGIN TREASUE PANE LAYOUT */
        //treasurePane.setLayout(new JavaGridLayout(2, 2, 4, 4));
        treasurePane.setBackground(backgroundColor);
        treasurePane.add(manaPane);
        treasurePane.add(goldPane);
        treasurePane.add(levelPane);
        treasurePane.add(gemsPane);

       // manaPane.setLayout(new BorderLayout(1, 0));
        manaPane.setBackground(highlightColor);
        manaImage.setSize(18, 18);
        manaPane.add("West", manaImage);
        manaPane.add("East", manaJavaLabel);
        manaPane.setLayout(new BorderLayout(1, 0));

        //goldPane.setLayout(new BorderLayout(1, 0));
        goldPane.setBackground(highlightColor);
        goldImage.setSize(18, 18);
        goldPane.add("West", goldImage);
        goldPane.add("East", goldJavaLabel);
        goldPane.setLayout(new BorderLayout(1, 0));

        //levelPane.setLayout(new BorderLayout(1, 0));
        levelPane.setBackground(highlightColor);
        levelImage.setSize(18, 18);
        levelPane.add("West", levelImage);
        levelPane.add("East", levelJavaLabel);
        levelPane.setLayout(new BorderLayout(1, 0));

        //gemsPane.setLayout(new BorderLayout(1, 0));
        gemsPane.setBackground(highlightColor);
        gemsImage.setSize(18, 18);
        gemsPane.add("West", gemsImage);
        gemsPane.add("East", gemsJavaLabel);
        gemsPane.setLayout(new BorderLayout(1, 0));
        treasurePane.setLayout(new JavaGridLayout(2, 2, 4, 4));
        /* END TREASUE PANE LAYOUT */

        /* BEGIN INVENTORY PANE LAYOUT */

        //equipmentPane.setLayout(new FlowLayout(FlowLayout.CENTER, 10, 0));

        cloakImage.setSize(24, 24);
        equipmentPane.add(cloakImage);

        blessingImage.setSize(24, 24);
        equipmentPane.add(blessingImage);

        crownImage.setSize(24, 24);
        equipmentPane.add(crownImage);

        palantirImage.setSize(24, 24);
        equipmentPane.add(palantirImage);

        ringImage.setSize(24, 24);
        equipmentPane.add(ringImage);

        virginImage.setSize(24, 24);
        equipmentPane.add(virginImage);
        equipmentPane.setLayout(new FlowLayout(FlowLayout.CENTER, 10, 0));
        /* END INVENTORY PANE LAYOUT */

    }

    internal void UpdateStatusPane(int thePacket)
    {
        switch (thePacket)
        {

            case constants.NAME_PACKET:
                name = parent.readString();
                if (name == null || name.Length == 0)
                {
                    titleCanvas.changeHead("Welcome to Phantasia!");
                    titleCanvas.changeTail("");
                }
                break;

            case constants.LOCATION_PACKET:
                titleCanvas.changeTail("( " + parent.readString() + " , " + parent.readString() + " )");
                location = parent.readString();
                if (name != null && name.Length > 0)
                {
                    titleCanvas.changeHead(name + " is in " + location); //todo
                }
                else
                {
                    titleCanvas.changeHead("Character starts at:");
                }
                break;

            case constants.ENERGY_PACKET:
                energyGraph.changeStats(parent.readString(), parent.readLong(), parent.readLong());
                break;

            case constants.STRENGTH_PACKET:
                strengthGraph.changeStats(parent.readString(), parent.readLong(), 0);
                break;

            case constants.SPEED_PACKET:
                quicknessGraph.changeStats(parent.readString(), parent.readLong(), 0);
                break;

            case constants.SHIELD_PACKET:
                shieldJavaLabel.changeStats(parent.readString());
                break;

            case constants.SWORD_PACKET:
                swordJavaLabel.changeStats(parent.readString());
                break;

            case constants.QUICKSILVER_PACKET:
                quicksilverJavaLabel.changeStats(parent.readString());
                break;

            case constants.MANA_PACKET:
                manaJavaLabel.changeStats(parent.readString());
                break;

            case constants.LEVEL_PACKET:
                levelJavaLabel.changeStats(parent.readString());
                break;

            case constants.GOLD_PACKET:
                goldJavaLabel.changeStats(parent.readString());
                break;

            case constants.GEMS_PACKET:
                gemsJavaLabel.changeStats(parent.readString());
                break;

            case constants.CLOAK_PACKET:
                cloakImage.setImage(7 + parent.readBool());
                break;

            case constants.BLESSING_PACKET:
                blessingImage.setImage(9 + parent.readBool());
                break;

            case constants.CROWN_PACKET:
                crownImage.setImage(11 + parent.readBool());
                break;

            case constants.PALANTIR_PACKET:
                palantirImage.setImage(13 + parent.readBool());
                break;

            case constants.RING_PACKET:
                ringImage.setImage(15 + parent.readBool());
                break;

            case constants.VIRGIN_PACKET:
                virginImage.setImage(17 + parent.readBool());
                break;

            default:
                parent.errorDialog.bringUp("statusPane told to update a non-existant item.",
                    "item: " + thePacket, "The game will now terminate.");
                break;
        }
        repaint();
    }

    internal void loadImages()
    {

        MediaTracker mt = new MediaTracker(this);

        /* load the images */
        parent.theImages[0] = parent.getImage(parent.getCodeBase(),
            "shield.gif");

        parent.theImages[1] = parent.getImage(parent.getCodeBase(),
            "sword.gif");

        parent.theImages[2] = parent.getImage(parent.getCodeBase(),
            "quicksilver.gif");

        parent.theImages[3] = parent.getImage(parent.getCodeBase(),
            "mana.gif");

        parent.theImages[4] = parent.getImage(parent.getCodeBase(),
            "level.gif");

        parent.theImages[5] = parent.getImage(parent.getCodeBase(),
            "gold.gif");

        parent.theImages[6] = parent.getImage(parent.getCodeBase(),
            "gems.gif");

        parent.theImages[7] = parent.getImage(parent.getCodeBase(),
            "cloak.No.gif");

        parent.theImages[8] = parent.getImage(parent.getCodeBase(),
            "cloak.Yes.gif");

        parent.theImages[9] = parent.getImage(parent.getCodeBase(),
            "blessing.No.gif");

        parent.theImages[10] = parent.getImage(parent.getCodeBase(),
            "blessing.Yes.gif");

        parent.theImages[11] = parent.getImage(parent.getCodeBase(),
            "crown.No.gif");

        parent.theImages[12] = parent.getImage(parent.getCodeBase(),
            "crown.Yes.gif");

        parent.theImages[13] = parent.getImage(parent.getCodeBase(),
            "palantir.No.gif");

        parent.theImages[14] = parent.getImage(parent.getCodeBase(),
            "palantir.Yes.gif");

        parent.theImages[15] = parent.getImage(parent.getCodeBase(),
            "ring.No.gif");

        parent.theImages[16] = parent.getImage(parent.getCodeBase(),
            "ring.Yes.gif");

        parent.theImages[17] = parent.getImage(parent.getCodeBase(),
            "virgin.No.gif");

        parent.theImages[18] = parent.getImage(parent.getCodeBase(),
            "virgin.Yes.gif");

        parent.theImages[19] = parent.getImage(parent.getCodeBase(),
            "label.1.gif");

        parent.theImages[20] = parent.getImage(parent.getCodeBase(),
            "label.2.gif");

        parent.theImages[21] = parent.getImage(parent.getCodeBase(),
            "label.3.gif");

        parent.theImages[22] = parent.getImage(parent.getCodeBase(),
            "label.4.gif");

        parent.theImages[23] = parent.getImage(parent.getCodeBase(),
            "label.5.gif");

        parent.theImages[24] = parent.getImage(parent.getCodeBase(),
            "label.6.gif");

        parent.theImages[25] = parent.getImage(parent.getCodeBase(),
            "label.7.gif");

        parent.theImages[26] = parent.getImage(parent.getCodeBase(),
            "label.8.gif");

        mt.addImage(parent.theImages[0], 0);
        mt.addImage(parent.theImages[1], 1);
        mt.addImage(parent.theImages[2], 2);
        mt.addImage(parent.theImages[3], 3);
        mt.addImage(parent.theImages[4], 4);
        mt.addImage(parent.theImages[5], 5);
        mt.addImage(parent.theImages[6], 6);
        mt.addImage(parent.theImages[7], 7);
        mt.addImage(parent.theImages[8], 8);
        mt.addImage(parent.theImages[9], 9);
        mt.addImage(parent.theImages[10], 10);
        mt.addImage(parent.theImages[11], 11);
        mt.addImage(parent.theImages[12], 12);
        mt.addImage(parent.theImages[13], 13);
        mt.addImage(parent.theImages[14], 14);
        mt.addImage(parent.theImages[15], 15);
        mt.addImage(parent.theImages[16], 16);
        mt.addImage(parent.theImages[17], 17);
        mt.addImage(parent.theImages[18], 18);
        mt.addImage(parent.theImages[19], 19);
        mt.addImage(parent.theImages[20], 20);
        mt.addImage(parent.theImages[21], 21);
        mt.addImage(parent.theImages[22], 22);
        mt.addImage(parent.theImages[23], 23);
        mt.addImage(parent.theImages[24], 24);
        mt.addImage(parent.theImages[25], 25);
        mt.addImage(parent.theImages[26], 26);

        try {
            mt.waitForAll(30000);
        }
        catch (InterruptedException e) 
        { };

        /* display the starting images */
        shieldImage.setImage(0);
        swordImage.setImage(1);
        quicksilverImage.setImage(2);
        manaImage.setImage(3);
        levelImage.setImage(4);
        goldImage.setImage(5);
        gemsImage.setImage(6);
        cloakImage.setImage(7);
        blessingImage.setImage(9);
        crownImage.setImage(11);
        palantirImage.setImage(13);
        ringImage.setImage(15);
        virginImage.setImage(17);

    }
}
