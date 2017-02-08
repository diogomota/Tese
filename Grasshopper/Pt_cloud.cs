private void RunScript(double Largura, int N_cabos, int Altura, double delta_h, double horiz_div, double subdiv, ref object Debug, ref object Cloud)
  {
    List<Point3d> Pt_cloud = new List<Point3d>(); // no programa final usar 2D array !!! [x,y,z]

    int x = 0;
    int y = 1;
    int h = 1;
    bool reverse = false;

    //#######################//
    //Tilt calc + N of rings //
    //#######################//
    double ring_z_step = (Altura) / horiz_div; // ok

    double tilt;
    tilt = (Largura * 0.5) / (Altura) * ring_z_step; //ok

    //##################//
    //main pt cloud loop//
    //##################//

    //Pts apoio
    Pt_cloud.Add(new Point3d(0, 0, 0));
    Pt_cloud.Add(new Point3d(Largura, 0, 0));
    Pt_cloud.Add(new Point3d(Largura, Largura, 0));
    Pt_cloud.Add(new Point3d(0, Largura, 0));

    for(h = 1; h <= horiz_div - 1; h++){ // ring loop

      double scale_factor = (1 - (h / horiz_div));
      double step = h * tilt;

      if(!reverse){ // x++ y++

        for(x = 0;x <= subdiv; x++){

          Pt_cloud.Add(new Point3d(step + x * (Largura / subdiv) * scale_factor, step, h * ring_z_step));

          if (x == subdiv){

            for(y = 1;y <= subdiv; y++){

              Pt_cloud.Add(new Point3d(Largura - step, step + y * (Largura / subdiv) * scale_factor, h * ring_z_step));

              if(x == subdiv && y == subdiv){reverse = true;} // start backwards loop
            }
          }
        }

      }

      if(reverse){ // x-- y--
        for(x = (int) subdiv - 1;x >= 0; x--){
          Pt_cloud.Add(new Point3d(step + x * (Largura / subdiv) * scale_factor, Largura - step, h * ring_z_step));

          if (x == 0){
            for(y = (int) subdiv - 1;y >= 1; y--){
              Pt_cloud.Add(new Point3d(step, step + y * (Largura / subdiv) * scale_factor, h * ring_z_step));


            }
          }
        }
      }

      reverse = false;
    }

    Cloud = Pt_cloud;
  }