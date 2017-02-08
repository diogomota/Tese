  private void RunScript(List<Point3d> Cloud, int subdiv, int horiz_div, ref object bars)
  {
    List<Line> Lines = new List<Line>();
    //
    //Connect support pts
    //
    for(int i = 0; i <= 4;i++){

      if (i == 0){
        for (int j = 4;j <= 4 + subdiv;j++){
          Lines.Add(new Line(Cloud[0], Cloud[j]));
        }
        for (int j = 8 + 4 * (subdiv - 1) - 1;j >= 8 + 4 * (subdiv - 1) - subdiv;j--){
          Lines.Add(new Line(Cloud[0], Cloud[j]));
        }

      }
      if (i == 1){
        for (int j = 4;j <= 4 + 2 * subdiv;j++){
          Lines.Add(new Line(Cloud[1], Cloud[j]));
        }
      }
      if (i == 2){
        for (int j = 4 + subdiv;j <= 4 + 3 * subdiv;j++){
          Lines.Add(new Line(Cloud[2], Cloud[j]));
        }
      }
      if (i == 3){
        for (int j = 4 + subdiv * 2;j <= 4 + 4 * subdiv - 1;j++){
          Lines.Add(new Line(Cloud[3], Cloud[j]));
        }
        Lines.Add(new Line(Cloud[3], Cloud[4]));
      }
    }

    //Connect all other points
    //add Lcr nesta fase !!!



    //TODO: Horizontal connections
    //add Lcr nesta fase!!!
    Lines.Add(new Line(Cloud[0], Cloud[1]));
    //
    bars = Lines;