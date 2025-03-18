using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class InvoiceService
    {
        #region Fields

        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;

        #endregion

        // Constructor for the InvoiceService class.
        internal InvoiceService(HogWildContext hogWildContext)
        {
            // Initialize the _hogWildContext field with the provided HogWildContext instance.
            _hogWildContext = hogWildContext;
        }

        //	Get the customer full name
        public string GetCustomerFullName(int customerID)
        {
            return _hogWildContext.Customers
                .Where(x => x.CustomerID == customerID)
                .Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
        }
        //	Get the employee full name
        public string GetEmployeeFullName(int employeeId)
        {
            {
                return _hogWildContext.Employees
                    .Where(x => x.EmployeeID == employeeId)
                    .Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
            }
        }
        // Get customer invoices
        public List<InvoiceView> GetCustomerInvoices(int customerId)
        {
            return _hogWildContext.Invoices
                .Where(x => x.CustomerID == customerId
                            && !x.RemoveFromViewFlag)
                .Select(x => new InvoiceView
                {
                    InvoiceID = x.InvoiceID,
                    InvoiceDate = x.InvoiceDate,
                    CustomerID = x.CustomerID,
                    SubTotal = x.SubTotal,
                    Tax = x.Tax
                }).ToList();
        }
        // Get invoice
        public InvoiceView GetInvoice(int invoiceID, int customerID, int employeeID)
        {
            // Business Rules
            // These are processing rules that need to be satisfied
            // for valid data

            // Rule: both the customerID and empplyeeID must be provided
            if (customerID == 0)
            {
                throw new ArgumentNullException("No customer was provided!");
            }

            if (employeeID == 0)
            {
                throw new ArgumentNullException("No employee was provided!");
            }
            //	Handles both new and existing invoice
            //  For a new invoice the following information is needed
            //		Customer & Employee ID
            //  For a existing invoice the following information is needed
            //		Invoice & Employee ID (We maybe updating an invoice at a later date
            //			and we need the current employee who is handling the transaction.

            InvoiceView invoice = new();
            //  new invoice for customer
            if (customerID > 0 && invoiceID == 0)
            {
                invoice = new InvoiceView
                {
                    CustomerID = customerID,
                    EmployeeID = employeeID,
                    InvoiceDate = DateTime.Now
                };
            }
            else
            {
                invoice = _hogWildContext.Invoices
                    .Where(x => x.InvoiceID == invoiceID
                     && !x.RemoveFromViewFlag
                    )
                    .Select(x => new InvoiceView
                    {
                        InvoiceID = invoiceID,
                        InvoiceDate = x.InvoiceDate,
                        CustomerID = x.CustomerID,
                        EmployeeID = x.EmployeeID,
                        SubTotal = x.SubTotal,
                        Tax = x.Tax,
                        InvoiceLines = _hogWildContext.InvoiceLines
                            .Where(invoiceLine => invoiceLine.InvoiceID == invoiceID)
                            .Select(invoiceLine => new InvoiceLineView
                            {
                                InvoiceLineID = invoiceLine.InvoiceLineID,
                                InvoiceID = invoiceLine.InvoiceID,
                                PartID = invoiceLine.PartID,
                                Quantity = invoiceLine.Quantity,
                                Description = invoiceLine.Part.Description,
                                Price = invoiceLine.Price,
                                Taxable = (bool)invoiceLine.Part.Taxable,
                                RemoveFromViewFlag = invoiceLine.RemoveFromViewFlag
                            }).ToList()
                    }).FirstOrDefault() ?? new InvoiceView();
                customerID = invoice.CustomerID;
            }
            invoice.CustomerName = GetCustomerFullName(customerID);
            invoice.EmployeeName = GetEmployeeFullName(employeeID);
            return invoice;
        }

        public InvoiceView Save(InvoiceView invoiceView)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data

            //  rule:	invoice cannot be null	
            if (invoiceView == null)
            {
                throw new ArgumentNullException("No invoice was supply");
            }

            //  rule:	invoice must have invoice lines	
            if (invoiceView.InvoiceLines.Count == 0)
            {
                throw new ArgumentNullException("Invoice must have invoice lines");
            }

            //  rule:	customer must be supply	
            if (invoiceView.CustomerID == 0)
            {
                throw new ArgumentNullException("No customer was provided!");
            }
            //  rule:   invoice line quantity must be greater than zero
            foreach (var invoiceLine in invoiceView.InvoiceLines)
            {
                if (invoiceLine.Quantity < 1)
                {
                    errorList.Add(new Exception($"Invoice line {invoiceLine.Description} has a value less than 1"));
                }
            }
            //  pre error check to exit if have any known error.  Exits before do intensive operations.
            //  post error check exists before SaveChanges
            if (errorList.Any())
            {
                //  we need to clear the "track changes" otherwise we leave
                //      our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                throw new AggregateException("Unable to save invoice.  Check Concerns", errorList);
            }
            #endregion

            #region Fetching Data and Setting Up References
            //  need to create a reference invoice lines for updating parts
            List<InvoiceLineView> referenceInvoiceLineViews = _hogWildContext.InvoiceLines
                .Where(x => x.InvoiceID == invoiceView.InvoiceID)
                .Select(x => new InvoiceLineView
                {
                    InvoiceLineID = x.InvoiceLineID,
                    InvoiceID = x.InvoiceID,
                    PartID = x.PartID,
                    Quantity = x.Quantity,
                    Description = x.Part.Description,
                    Price = x.Price,
                    Taxable = (bool)x.Part.Taxable,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).ToList();

            //  get a list of parts.  This will be use to update quantity on hand (QOH)
            List<Part> parts = _hogWildContext.Parts
                .Select(x => x).ToList();

            //  get the current invoice from the database
            Invoice? invoice = _hogWildContext.Invoices
                .Where(x => x.InvoiceID == invoiceView.InvoiceID)
                .Select(x => x).FirstOrDefault();

            //  get a list of invoice lines from the database for use in comparing.
            List<InvoiceLine> invoiceLines = _hogWildContext.InvoiceLines
                .Where(x => x.InvoiceID == invoiceView.InvoiceID)
                .Select(x => x).ToList();
            #endregion

            #region Invoice Existence Check and Initialization
            //  invoice does not exist (new)
            //  creating an instance of the invoice (stage)
            if (invoice == null)
            {
                invoice = new Invoice();
            }
            //  update invoice properties.
            invoice.InvoiceDate = invoiceView.InvoiceDate;
            invoice.CustomerID = invoiceView.CustomerID;
            invoice.EmployeeID = invoiceView.EmployeeID;
            #endregion

            #region Processing Invoice Lines
            foreach (var invoiceLineView in invoiceView.InvoiceLines)
            {
                //  existing invoice line
                if (invoiceLineView.InvoiceLineID > 0)
                {
                    InvoiceLine? invoiceLine = invoice.InvoiceLines
                        .Where(x => x.InvoiceLineID == invoiceLineView.InvoiceLineID)
                        .Select(x => x).FirstOrDefault();
                    if (invoiceLine == null)
                    {
                        string missingInvoiceLine = $"Invoice line for {invoiceLineView.Description} ";
                        missingInvoiceLine = missingInvoiceLine + "cannot be found in the existing invoice lines";
                        errorList.Add(new Exception(missingInvoiceLine));
                    }
                    else
                    {
                        invoiceLine.Quantity = invoiceLineView.Quantity;
                        invoiceLine.RemoveFromViewFlag = invoiceLineView.RemoveFromViewFlag;
                    }
                }
                else
                {
                    //  new invoice line
                    InvoiceLine invoiceLine = new();
                    invoiceLine.PartID = invoiceLineView.PartID;
                    invoiceLine.Quantity = invoiceLineView.Quantity;
                    invoiceLine.Price = invoiceLineView.Price;
                    invoiceLine.RemoveFromViewFlag = invoiceLineView.RemoveFromViewFlag;
                    //  update part QOH
                    Part? part = parts.Where(x => x.PartID == invoiceLineView.PartID)
                        .Select(x => x).FirstOrDefault();
                    if (part != null)
                    {
                        part.QOH = part.QOH - invoiceLineView.Quantity;
                        //  updated the parts.
                        _hogWildContext.Parts.Update(part);
                    }

                    //	What about the second part of the primary key:  InvoiceID?
                    //	IF invoice exists, then we know the id:  invoice.InvoiceId
                    //	But if the invoice is NEW, we DO NOT KNOW the id

                    //	In the situation of a NEW invoice, even though we have created the 
                    //		invoice instance (see above), it is ONLY stage (In memory)
                    //	This means that the actual SQL record has NOT yet been created.
                    //	This means that the IDENTITY value for the new invoice DOES NOT yet exists.
                    //	The value on the invoice instance (invoice lines) is zero(0).
                    //		Therefore, we have a serious problem.

                    //	Solution
                    //	It is build into the Entity Framework software and is based on using the
                    //		navigational property in the invoice pointing to it's "child"

                    //	Staging a typical Add in the past was to reference the entity and
                    //		use the entity.Add(xxx)
                    //	If you use this statement, the invoiceID would be zero (0)
                    //		causing your transaction to ABORT.
                    //	Why?	PKeys cannot be zero (0) (FKey to PKey problem)

                    //	Instead, do the staging using the "parent.navChildProperty.Add(xxx)
                    invoice.InvoiceLines.Add(invoiceLine);
                }
            }
            #endregion

            #region  Update parts quantity on hand (QOH)
            foreach (var invoiceLineView in invoiceView.InvoiceLines)
            {
                //  get the part that we might be updating
                Part? part = parts.Where(x => x.PartID == invoiceLineView.PartID)
                    .Select(x => x).FirstOrDefault();

                //  get the invoice line that is stored in the database.
                InvoiceLineView? referenceInvoiceLineView = referenceInvoiceLineViews
                    .Where(x => x.InvoiceLineID == invoiceLineView.InvoiceLineID)
                    .Select(x => x).FirstOrDefault();


                if (referenceInvoiceLineView != null && part != null)
                {
                    //  check to see if a change has occur for the quantity
                    if (referenceInvoiceLineView.Quantity != invoiceLineView.Quantity)
                    {
                        //  logic
                        //  part QOH = 10
                        //  current invoice line view quantity is 5
                        //  previous invoice line (reference) quantity is 4
                        //  this means that we have sold 1 more item (5 - 4 = 1)
                        //  10 - (5 - 4)[1] => 9 items on hand.
                        //========================================
                        //  if we sold less item now than before, we would be adding quantity back to the inventory
                        //  part QOH = 10
                        //  current invoice line view quantity is 4
                        //  previous invoice line (reference) quantity is 5
                        //  this means that we have sold 1 less item (4 - 5 = -1)
                        //  10 - (4 - 5)[-1] => 11 items on hand.
                        part.QOH = part.QOH - (referenceInvoiceLineView.Quantity - invoiceLineView.Quantity);
                        //  updated the parts.
                        _hogWildContext.Parts.Update(part);
                    }
                }
            }
            #endregion

            #region  Remove any lines that have been deleted
            foreach (var referenceInvoiceLine in referenceInvoiceLineViews)
            {
                //  check to see if we have invoice lines
                //      that are not in invoiceView.InvoiceLines (InvoiceLineID)
                if (!invoiceView.InvoiceLines.Any(x => x.InvoiceLineID == referenceInvoiceLine.InvoiceLineID))
                {
                    //  get the part that is being updating
                    Part? part = parts.Where(x => x.PartID == referenceInvoiceLine.PartID)
                        .Select(x => x).FirstOrDefault();
                    //  logic
                    //  part QOH = 10
                    //  current invoice line quantity is 5
                    //  we are now going to add back the items to the quantity on hand (QOH)
                    //  10 + 5  => 15 items on hand.
                    if (part != null)
                    {
                        part.QOH = part.QOH + referenceInvoiceLine.Quantity;
                        //  updated the parts.
                        _hogWildContext.Parts.Update(part);
                    }

                    //  remove the invoice line from the database
                    InvoiceLine? deletedInvoiceLine = _hogWildContext.InvoiceLines
                        .Where(x => x.InvoiceLineID == referenceInvoiceLine.InvoiceLineID)
                        .Select(x => x).FirstOrDefault();
                    if (deletedInvoiceLine != null)
                        _hogWildContext.InvoiceLines.Remove(deletedInvoiceLine);
                }
            }
            #endregion

            #region Update Subtotal and Tax
            invoice.SubTotal = 0;
            invoice.Tax = 0;
            foreach (var invoiceLineView in invoice.InvoiceLines)
            {
                invoice.SubTotal = invoice.SubTotal + (invoiceLineView.Quantity
                                                               * invoiceLineView.Price);
                //	we need to get the part in-case we are creating a new invoice line
                bool isTaxable = _hogWildContext.Parts
                    .Where(x => x.PartID == invoiceLineView.PartID)
                    .Select(x => (bool)x.Taxable)
                    .FirstOrDefault();
                invoice.Tax = invoice.Tax + (isTaxable
                                                ? invoiceLineView.Quantity * invoiceLineView.Price * .05m
                                                : 0);
            }
            #endregion

            //  new employee
            if (invoice.InvoiceID == 0)
                _hogWildContext.Invoices.Add(invoice);
            else
                _hogWildContext.Invoices.Update(invoice);

            #region Final Error Check and Save Operation
            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add or edit invoice. Please check error message(s)",
                                                errorList);
            }
            else
            {
                _hogWildContext.SaveChanges();
            }
            #endregion

            return GetInvoice(invoice.InvoiceID, invoice.CustomerID, invoice.EmployeeID);
        }
    }
}
