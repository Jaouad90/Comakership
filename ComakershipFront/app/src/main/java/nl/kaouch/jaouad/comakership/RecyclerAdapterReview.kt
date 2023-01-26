package nl.kaouch.jaouad.comakership

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.models.Review

class RecyclerAdapterReview (
            private val context: Context,
            private var reviews: List<Review>?
    ): RecyclerView.Adapter<RecyclerAdapterReview.ViewHolder>() {

        override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerAdapterReview.ViewHolder {
            val comakershipView = LayoutInflater.from(context).inflate(
                    R.layout.card_layout_reviews_company,
                    parent,
                    false
            )
            return ViewHolder(comakershipView)
        }

        override fun onBindViewHolder(holder: RecyclerAdapterReview.ViewHolder, position: Int) {

            if (!reviews!!.isNullOrEmpty()) {
                holder.reviewerName.text = reviews!![position].reviewersName
                holder.reviewerRate.text = reviews!![position].rating.toString()+"/10"
                holder.reviewerComment.text = reviews!![position].comment
            } else {
                holder.reviewerName.text = "No existing reviews found!!"
            }
        }

        override fun getItemCount(): Int {
            if (reviews != null) {
                if (!reviews.isNullOrEmpty()) {
                    return reviews!!.size
                }
            }
            return -1
        }

        inner class ViewHolder(reviewView: View): RecyclerView.ViewHolder(reviewView){

            var reviewerName: TextView = itemView.findViewById(R.id.reviewer_name_value)
            var reviewerRate: TextView = itemView.findViewById(R.id.reviewer_rate_value)
            var reviewerComment: TextView = itemView.findViewById(R.id.reviewer_comment_value)

        }
    }