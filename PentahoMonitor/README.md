
## The Monitis Custom Monitor with Pentaho ##

The presented custom monitor built with [Pentaho Data Integration suite](http://kettle.pentaho.com/) for  
monitoring/sharing results of SQL query run on test database.  

The project contains the following files  

  - authentication.ktr – transformation implementing authentication token  
    request providing API and secret keys  
  - create_cm.ktr – transformation for creating monitor with given Name,  
    Tag, Type and Parameters description  

  - results_cm.ktr – transformation, responsible for posting monitoring results
  - results_cm.kjb – job for results_cm.ktr
